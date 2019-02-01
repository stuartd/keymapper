using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using KeyMapper.Classes.Interop;
using KeyMapper.Providers;
using Microsoft.Win32;

namespace KeyMapper.Classes
{
    public static class MappingsManager
    {
        // Saved mapping data
        private static Collection<KeyMapping> savedBootMappings = new Collection<KeyMapping>();

        // Mapping data
        private static Collection<KeyMapping> mappings = new Collection<KeyMapping>();

        // Current mappings based on current filter
        private static readonly Collection<KeyMapping> currentFilteredMappings = new Collection<KeyMapping>();

        // Need to maintain a collection of mappings which have been cleared
        // (ie which existed at boot or logon but don't exist now)
        private static readonly Collection<KeyMapping> clearedBootMappings = new Collection<KeyMapping>();

        // If user has existing mappings on first run, store them so 
        // new mappings can be distinguished from them.
        private static Collection<KeyMapping> unsavedMappings = new Collection<KeyMapping>();

        // Undo/Redo stacks are implemented as pairs.
        private static readonly UndoRedoMappingStack undostack = new UndoRedoMappingStack();
        private static readonly UndoRedoMappingStack redostack = new UndoRedoMappingStack();

        public static int UndoStackCount => undostack.Count;

        public static int RedoStackCount => redostack.Count;

        public static IEnumerable<KeyMapping> ClearedMappings => clearedBootMappings;

        public static void StoreUnsavedMappings()
        {
            unsavedMappings = CopyMappings(mappings);
        }

        private static void PopulateMappingLists()
        {
            // Populate the internal mapping lists at startup and when mappings change.
            currentFilteredMappings.Clear();
            clearedBootMappings.Clear();

            foreach (var bootmap in mappings)
            {
                currentFilteredMappings.Add(bootmap);
            }

            // Finally, skip through the user and boot mappings so we can populate the cleared mappings lists
            foreach (var map in savedBootMappings)
            {
                if (mappings.Contains(map) == false) {
					clearedBootMappings.Add(map);
				}
			}
        }


        private static Collection<KeyMapping> CopyMappings(Collection<KeyMapping> keyMappings)
        {
            var copy = new Collection<KeyMapping>();

            foreach (var map in keyMappings.Where(map => map.IsValid()))
            {
                copy.Add(map);
            }

            return copy;
        }

        public static Collection<KeyMapping> GetMappings(MappingFilter filter)
        {
            switch (filter)
            {
                case MappingFilter.Current:
                    return currentFilteredMappings;

                case MappingFilter.Boot:
                    return mappings;

                case MappingFilter.ClearedBoot:
                    return clearedBootMappings;

                default:
                    throw new NotImplementedException();
            }
        }

        public static int GetMappingCount(MappingFilter filter)
        {
            switch (filter)
            {
                case MappingFilter.Current:
                    return currentFilteredMappings.Count;

                case MappingFilter.Boot:
                    return mappings.Count;

                case MappingFilter.ClearedBoot:
                    return clearedBootMappings.Count;

                default:
                    return 0;
            }
        }

        public static bool IsRestartRequired()
        {
            if (clearedBootMappings.Count != 0) {
				return true;
			}

			// Need to iterate through to see if any have changed.

            foreach (var km in mappings)
            {
                if (savedBootMappings.Contains(km) == false && unsavedMappings.Contains(km) == false) {
					return true;
				}
			}

            return false;
        }

        public static bool VistaMappingsNeedSaving()
        {
            // Check whether the current boot mappings and the proposed boot mappings 
            // are the same: if not, they need saving

            var map = RegistryProvider.GetScanCodeMapFromRegistry(MapLocation.KeyMapperVistaMappingsCache);

            if (map == null) {
				return false;
			}

			var maps = GetMappingsFromScanCodeMap(map);

            if (maps.Count != mappings.Count) {
				return true;
			}

			foreach (var km in maps)
            {
                if (mappings.Contains(km) == false) {
					return true;
				}
			}

            return false;
        }

        public static bool IsMapped(KeyMapping map)
        {
            return mappings.Contains(map);
        }

        public static bool IsMappingPending(KeyMapping map, MappingFilter filter = MappingFilter.Boot)
        {
            if (unsavedMappings.Contains(map)) {
				return false;
			}

			// Did this mapping exist at boot or logon time?
            switch (filter)
            {
                case MappingFilter.ClearedBoot:
                    return true; // A cleared mapping is by definition pending

                case MappingFilter.Boot:
                    return !(savedBootMappings.Contains(map));
            }

            return true;
        }

        public static KeyMapping GetKeyMapping(int scanCode, int extended)
        {
            foreach (var mapping in currentFilteredMappings)
            {
                if (mapping.From.ScanCode == scanCode && mapping.From.Extended == extended) {
					return mapping;
				}
			}

            return GetEmptyMapping(new Key(scanCode, extended));
        }

        public static KeyMapping GetClearedMapping(int scanCode, int extended)
        {
            foreach (var mapping in clearedBootMappings)
            {
                if (mapping.From.ScanCode == scanCode && mapping.From.Extended == extended)
                {
                    return mapping;
                }
            }

            return GetEmptyMapping(new Key(scanCode, extended));
        }

        public static KeyMapping GetEmptyMapping(Key from)
        {
            return new KeyMapping(from, new Key(-1, -1));
        }

        public static bool IsEmptyMapping(KeyMapping map)
        {
            return (map.To.ScanCode == -1 && map.To.Extended == -1);
        }

        public static bool IsDisabledMapping(KeyMapping map)
        {
            return (map.To.ScanCode == 0 && map.To.Extended == 0);
        }

        public static void SaveBootMappingsVista()
        {
            // Well, we need to write to HKLM under Vista or later.
            // Create a registry file and run it, user will have to allow regedit to run.

            if (!AppController.ConfirmWriteToProtectedSectionOfRegistryOnVistaOrLater("the changes to your boot mappings"))
            {
                return;
            }

            string tempfile = ExportMappingsAsRegistryFile(true);

            AppController.WriteRegistryFileVista(tempfile);
        }

        public static void SaveBootMappingsToKeyMapperKey()
        {
            SaveMappings(Mappings.CurrentBootMappings, MapLocation.KeyMapperLocalMachineKeyboardLayout);

            // As have overwritten our stored value with a new one, reload it ...
            GetMappingsFromRegistry(MapLocation.KeyMapperLocalMachineKeyboardLayout);

            // ... and recalculate mappings.
            PopulateMappingLists();
        }

        public static void SaveMappings(Mappings whichMappings = Mappings.CurrentBootMappings, MapLocation whereToSave = MapLocation.LocalMachineKeyboardLayout)
        {
            Collection<KeyMapping> maps;

            switch (whichMappings)
            {
                case Mappings.CurrentBootMappings:
                    maps = mappings;
                    break;
                case Mappings.SavedBootMappings:
                    maps = savedBootMappings;
                    break;
                default:
                    return;
            }

            RegistryHive hive = 0;
            string keyName = "", valueName = "";

            RegistryProvider.GetRegistryLocation(whereToSave, ref hive, ref keyName, ref valueName);

            RegistryKey registry = null;

            // Need write access so prepare to fail 
            try
            {
                if (hive == RegistryHive.LocalMachine)
                {
                    registry = Registry.LocalMachine.OpenSubKey(keyName, true);
                }
                else if (hive == RegistryHive.CurrentUser)
                {
                    registry = Registry.CurrentUser.OpenSubKey(keyName, true);
                }
            }
            catch (SecurityException ex)
            {
                if (hive == RegistryHive.CurrentUser)
                {
                    // Would expect to be able to write to HKCU
                    Console.WriteLine("Unexpected failure {2} opening {0} on {1} for write access", keyName, Enum.GetNames(typeof(Mappings))[(int)whichMappings], ex.Message);
                }

                return;
            }

            if (registry == null)
            {
                // Key doesn't exist. TODO Shouldn't this be logged?
                return;
            }

            if (maps.Count == 0)
            {
                registry.SetValue(valueName, new byte[0]);
            }
            else
            {
                registry.SetValue(valueName, GetMappingsAsByteArray(maps));
            }
        }

        private static byte[] GetMappingsAsByteArray(Collection<KeyMapping> maps)
        {
            // Turn mappings into a byte[] 
            int count = maps.Count;
            int size = (16 + (count * 4));

            // Check they are all zero.

            var bytemappings = new byte[size];

            // Allow for the null mapping at the end
            bytemappings[8] = (byte)(count + 1);

            const int start = 12;

            for (int i = 0; i < count; i++)
            {
                // Make sure we don't extend beyond array bounds
                if (size <= (start + (i * 4) + 3))
                {
                    break;
                }

                var map = maps[i];

                // First pair is the action - what the mapped key does.
                bytemappings[start + (i * 4)] = (byte)map.To.ScanCode;
                bytemappings[start + (i * 4) + 1] = (byte)map.To.Extended;

                // Second pair is the physical key which performs the new action
                bytemappings[start + (i * 4) + 2] = (byte)map.From.ScanCode;
                bytemappings[start + (i * 4) + 3] = (byte)map.From.Extended;
            }

            return bytemappings;
        }

        public static string ExportMappingsAsRegistryFile(bool useTempFile)
        {
            // This is the required format:

            // Windows Registry Editor Version 5.00

            // [HKEYLOCALMACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]
            // "ScanCode Map"=hex:00,00,00,00,00,00,00,00,02,00,00,00,2a,00,3a,00,00,00,00,00

            // [HKEYCURRENTUSER\Keyboard Layout]
            // "ScanCode Map"=hex:00,00,00,00,00,00,00,00,04,00,00,00,5d,e0,1c,e0,1d,00,5b,e0,2a,00,3a,00,00,00,00,00

            // Where there are no mappings, delete the value:
            // "ScanCode Map"=-

            string filename;

            if (useTempFile) {
				filename = Path.GetTempPath() + Path.GetRandomFileName() + ".reg";
			}
			else
            {
                var fd = new SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = "reg",
                    Filter = "Registry files (*.reg)|*.reg",
                    InitialDirectory =
                                     Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    OverwritePrompt = true,
                    FileName = "Key Mappings",
                    AutoUpgradeEnabled = true
                };

                var dr = fd.ShowDialog();

                if (dr != DialogResult.OK)
                {
                    return string.Empty;
                }

                filename = fd.FileName;
            }

            int bootMappingCount = GetMappingCount(MappingFilter.Boot);

            using (var sw = new StreamWriter(filename, false, Encoding.Unicode))
            {
                sw.WriteLine("Windows Registry Editor Version 5.00");
                sw.WriteLine();
                sw.WriteLine(@"[HKEYLOCALMACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]");
                sw.Write("\"ScanCode Map\"=");
                if (bootMappingCount > 0)
                {
                    sw.Write("hex:");
                    WriteMappingsToStream(sw, GetMappingsAsByteArray(GetMappings(MappingFilter.Boot)));
                }
                else
                {
                    sw.Write("-");
                }

                sw.WriteLine();

                if (bootMappingCount > 0) {
					sw.WriteLine();
				}
			}

            return filename;
        }

        private static void WriteMappingsToStream(StreamWriter sw, byte[] bytemappings)
        {
            for (int i = 0; i < bytemappings.GetLength(0); i++)
            {
                sw.Write(bytemappings[i].ToString("X", CultureInfo.InvariantCulture).PadLeft(2, (char)48));
                if (i < bytemappings.GetLength(0) - 1) {
					sw.Write(",");
				}
			}
        }

        private static void RaiseMappingsChangedEvent()
        {
            PopulateMappingLists();
            SaveMappings();
            MappingsChanged?.Invoke(null, null);
        }

        public static bool AddMapping(KeyMapping map)
        {
            return AddMapping(map, false);
        }

        private static bool AddMapping(KeyMapping map, bool noStackNoEventRaised)
        {
            if (!map.IsValid())
            {
                return false;
            }

            int scanCode = map.From.ScanCode;
            int extended = map.From.Extended;

            // If user is remapping Left Ctrl, Left Alt, or Delete then s/he must confirm
            // that it could be goodbye to CTRL-ALT-DEL

            if ((scanCode == 29 && extended == 0) || (scanCode == 56 && extended == 0) ||
                (scanCode == 83 && extended == 224))
            {
                string action = IsDisabledMapping(map) ? "disable " : "remap ";

                string warning = "You are attempting to " + action + map.From.Name +
                                 " which is required for CTRL-ALT-DELETE." + (char)13 +
                                 "If you continue you may not be able to log on" +
                                 " to your PC.";

                string question = "Are you really sure you want to " + action + "this key?";


                var dr = FormsManager.ShowTaskDialog(question, warning, "Key Mapper",
                                                                  TaskDialogButtons.Yes | TaskDialogButtons.No,
                                                                  TaskDialogIcon.Question);
                if (dr != TaskDialogResult.Yes) {
					return false;
				}
			}

            // If user is remapping Pause, then suggest they will want to disable Num Lock as well.

            bool disableNumLock = false;

            if (scanCode == 29 && extended == 225 && IsDisabledMapping(map) == false)
            {
                // Is Num Lock already disabled or remapped?
                bool numLockIsDisabled = false;
                bool numLockIsMapped = false;

                foreach (var km in mappings) {
					if (km.From.ScanCode == 69)
					{
						if (IsDisabledMapping(km)) {
							numLockIsDisabled = true;
						}
						else {
							numLockIsMapped = true;
						}
					}
				}

				if (numLockIsDisabled == false)
                {
                    string warning = "If you remap Pause, the Num Lock key will be disabled" +
                                     (numLockIsMapped
                                          ? ((char)13 + "and your existing Num Lock mapping will be removed.")
                                          : ".");

                    const string question = "Do you still want to remap Pause?";


                    var dr = FormsManager.ShowTaskDialog(question, warning, "Key Mapper",
                                                                      TaskDialogButtons.Yes | TaskDialogButtons.No,
                                                                      TaskDialogIcon.Question);
                    if (dr != TaskDialogResult.Yes) {
						return false;
					}

					disableNumLock = true;
                }
            }

            if (noStackNoEventRaised == false) {
				PushMappingsOntoUndoStack();
			}

			// Check for any existing mappings for this key
            // if they exist, this mapping needs to replace them.

            var existingMap = GetKeyMapping(map.From.ScanCode, map.From.Extended);

            if (existingMap.IsEmpty() == false) {
				mappings.Remove(existingMap);
			}

			mappings.Add(map);

            if (disableNumLock)
            {
                var nl = new KeyMapping(new Key(69, 0), new Key(0, 0));
                AddMapping(nl, true);
            }

            if (noStackNoEventRaised == false) {
				RaiseMappingsChangedEvent();
			}

			return true;
        }

        public static void DeleteMapping(KeyMapping map)
        {
            if (!map.IsValid()) {
				throw new ArgumentException("Can't delete an invalid map");
			}

			PushMappingsOntoUndoStack();

            if (mappings.Contains(map))
            {
                mappings.Remove(map);
            }

            RaiseMappingsChangedEvent();
        }

        public static void ClearMappings()
        {
            PushMappingsOntoUndoStack();

            mappings = new Collection<KeyMapping>();

            RaiseMappingsChangedEvent();
        }

        public static void RevertToStartupMappings()
        {
            PushMappingsOntoUndoStack();
            mappings = CopyMappings(savedBootMappings);
            RaiseMappingsChangedEvent();
        }

        public static void UndoMappingChange()
        {
            if (undostack.Count < 1) {
				return;
			}

			PushMappingsOntoRedoStack();
            PopMappingsOffUndoStack();
            RaiseMappingsChangedEvent();
        }

        public static void RedoMappingChange()
        {
            if (redostack.Count < 1) {
				return;
			}

			// To 'redo', the latest entry on the redo stack is popped into current members
            // which is itself popped onto the undo stack.
            PushMappingsOntoUndoStack();
            PopMappingsOffRedoStack();
            RaiseMappingsChangedEvent();
        }

        private static Collection<KeyMapping> GetMappingsFromScanCodeMap(byte[] map)
        {
            // Transform the byte array into keymappings
            var maps = new Collection<KeyMapping>();

            int count = 0;
            int length = map.GetLength(0);

            // How many mappings are there?
            // (Make sure there are at least 8 bytes in the array)

            if (length > 8) {
				count = map[8] - 1;
			}

			if (count == 0)
            {
                return maps;
            }

            const int start = 12;

            for (int i = 0; i < count; i++)
            {
                // Make sure we don't extend beyond array bounds
                if (length >= (start + (i * 4) + 3))
                {
                    // First pair is the action - what the mapped key does.
                    int word1 = map[start + (i * 4)];
                    int word2 = map[start + (i * 4) + 1];

                    var tokey = new Key(word1, word2);

                    // Second pair is the physical key which performs the new action
                    word1 = map[start + (i * 4) + 2];
                    word2 = map[start + (i * 4) + 3];

                    var fromkey = new Key(word1, word2);

                    var mapping = new KeyMapping(fromkey, tokey);

                    if (mapping.IsValid())
                    {
                        maps.Add(mapping);
                    }
                    else
                    // Just ignore it and hope it goes away.
                    // A manually added - or garbled - entry could be invalid.
                    {
                    }
                }
            }

            return maps;
        }

        public static void GetMappingsFromRegistry()
        {
            GetMappingsFromRegistry(MapLocation.LocalMachineKeyboardLayout);
            GetMappingsFromRegistry(MapLocation.KeyMapperLocalMachineKeyboardLayout);
            PopulateMappingLists();
        }

        private static void GetMappingsFromRegistry(MapLocation location)
        {
            var mappings = new Collection<KeyMapping>();

            var map = RegistryProvider.GetScanCodeMapFromRegistry(location);

            if (map != null) {
				mappings = GetMappingsFromScanCodeMap(map);
			}

			switch (location)
            {
                case MapLocation.LocalMachineKeyboardLayout:
                    MappingsManager.mappings = mappings;
                    break;
                case MapLocation.KeyMapperLocalMachineKeyboardLayout:
                    savedBootMappings = mappings;
                    break;
                default:
                    throw new InvalidOperationException("Unknown value for MapLocation: " + location);
            }
        }

        public static event EventHandler<EventArgs> MappingsChanged;

        private static void PushMappingsOntoUndoStack()
        {
            undostack.Push(CopyMappings(mappings));
        }

        private static void PushMappingsOntoRedoStack()
        {
            redostack.Push(CopyMappings(mappings));
        }

        private static void PopMappingsOffUndoStack()
        {
            mappings = undostack.BootStack.Pop();
        }

        private static void PopMappingsOffRedoStack()
        {
            mappings = redostack.BootStack.Pop();
        }
    }
}