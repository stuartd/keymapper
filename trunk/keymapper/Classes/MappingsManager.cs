using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
        private static Collection<KeyMapping> _savedBootMappings = new Collection<KeyMapping>();
        private static Collection<KeyMapping> _savedUserMappings = new Collection<KeyMapping>();

        // Mapping data
        private static Collection<KeyMapping> _bootMappings = new Collection<KeyMapping>();
        private static Collection<KeyMapping> _userMappings = new Collection<KeyMapping>();
        private static readonly Collection<KeyMapping> _allMappings = new Collection<KeyMapping>();

        // Current mappings based on current filter
        private static readonly Collection<KeyMapping> currentFilteredMappings = new Collection<KeyMapping>();

        // Need to maintain two collections of mappings which have been cleared
        // (ie which existed at boot or logon but don't exist now)
        private static readonly Collection<KeyMapping> clearedUserMappings = new Collection<KeyMapping>();
        private static readonly Collection<KeyMapping> clearedBootMappings = new Collection<KeyMapping>();

        // If user has existing mappings on first run, store them so 
        // new mappings can be distinguished from them.
        private static Collection<KeyMapping> _unsavedMappings = new Collection<KeyMapping>();

        // Undo/Redo stacks are implemented as pairs.
        private static readonly UndoRedoMappingStack undostack = new UndoRedoMappingStack();
        private static readonly UndoRedoMappingStack redostack = new UndoRedoMappingStack();

        private static readonly IOperatingSystemCapability operatingSystemCapability = new OperatingSystemCapabilityProvider();

        static MappingsManager()
        {
            Filter = MappingFilter.All;
        }

        public static int UndoStackCount
        {
            get { return undostack.Count; }
        }

        public static int RedoStackCount
        {
            get { return redostack.Count; }
        }

        public static MappingFilter Filter { get; private set; }


        public static IEnumerable<KeyMapping> ClearedMappings
        {
            get
            {
                switch (Filter)
                {
                    case MappingFilter.All:
                        Collection<KeyMapping> temp = CopyMappings(clearedBootMappings);
                        foreach (KeyMapping km in clearedUserMappings)
                        {
                            temp.Add(km);
                        }
                        return temp;

                    case MappingFilter.Boot:
                        return clearedBootMappings;

                    case MappingFilter.User:
                        return clearedUserMappings;

                    default:
                        return new Collection<KeyMapping>();
                }
            }
        }

        #region Mapping utility methods

        public static void StoreUnsavedMappings()
        {
            _unsavedMappings = CopyMappings(_bootMappings);
            foreach (KeyMapping km in _userMappings)
                _unsavedMappings.Add(km);
        }

        public static void PopulateMappingLists()
        {
            // Populate the internal mapping lists at startup and when mappings change.
            currentFilteredMappings.Clear();
            clearedUserMappings.Clear();
            clearedBootMappings.Clear();
            _allMappings.Clear();


            foreach (KeyMapping bootmap in _bootMappings)
            {
                // Add everything to All Mappings
                _allMappings.Add(bootmap);

                bool addToFilteredMappings = true;

                switch (Filter)
                {
                    case MappingFilter.All:
                        // If filter is All then need to check if this boot mapping is overriden by a user mapping 
                        // Because if it is, we don't add it.

                        foreach (KeyMapping usermap in _userMappings)
                        {
                            if (bootmap.From == usermap.From)
                            {
                                addToFilteredMappings = false;
                                break;
                            }
                        }

                        break;

                    case MappingFilter.User:
                        addToFilteredMappings = false;
                        break;
                }

                if (addToFilteredMappings)
                    currentFilteredMappings.Add(bootmap);
            }

            foreach (KeyMapping usermap in _userMappings)
            {
                _allMappings.Add(usermap);

                if (Filter == MappingFilter.All || Filter == MappingFilter.User)
                    currentFilteredMappings.Add(usermap);
            }

            // Finally, skip through the user and boot mappings so we can populate the cleared mappings lists
            foreach (KeyMapping map in _savedUserMappings)
            {
                if (_userMappings.Contains(map) == false)
                    clearedUserMappings.Add(map);
            }

            foreach (KeyMapping map in _savedBootMappings)
            {
                if (_bootMappings.Contains(map) == false)
                    clearedBootMappings.Add(map);
            }
        }


        private static Collection<KeyMapping> CopyMappings(Collection<KeyMapping> mappings)
        {
            var copy = new Collection<KeyMapping>();

            foreach (KeyMapping map in mappings)
            {
                if (map.IsValid())
                    copy.Add(map);
            }
            return copy;
        }

        #endregion

        #region Public methods

        public static Collection<KeyMapping> GetMappings(MappingFilter filter)
        {
            switch (filter)
            {
                case MappingFilter.All:
                    return _allMappings;

                case MappingFilter.Current:
                    return currentFilteredMappings;

                case MappingFilter.Boot:
                    return _bootMappings;

                case MappingFilter.User:
                    return _userMappings;

                case MappingFilter.ClearedUser:
                    return clearedUserMappings;

                case MappingFilter.ClearedBoot:
                    return clearedBootMappings;
            }

            return null;
        }

        public static int GetMappingCount(MappingFilter filter)
        {
            switch (filter)
            {
                case MappingFilter.All:
                    return _allMappings.Count;

                case MappingFilter.Current:
                    return currentFilteredMappings.Count;

                case MappingFilter.Boot:
                    return _bootMappings.Count;

                case MappingFilter.User:
                    return _userMappings.Count;

                case MappingFilter.ClearedUser:
                    return clearedUserMappings.Count;

                case MappingFilter.ClearedBoot:
                    return clearedBootMappings.Count;

                default:
                    return 0;
            }
        }

        public static bool IsRestartRequired()
        {
            if (clearedBootMappings.Count != 0)
                return true;

            // Need to iterate through to see if any have changed.

            foreach (KeyMapping km in _bootMappings)
            {
                if (_savedBootMappings.Contains(km) == false && _unsavedMappings.Contains(km) == false)
                    return true;
            }

            return false;
        }

        public static bool IsLogOnRequired()
        {
            if (clearedUserMappings.Count != 0)
                return true;

            // Need to look for new mappings as well.
            foreach (KeyMapping km in _userMappings)
            {
                if (_savedUserMappings.Contains(km) == false && _unsavedMappings.Contains(km) == false)
                    return true;
            }

            return false;
        }

        public static bool VistaMappingsNeedSaving()
        {
            // Check whether the current boot mappings and the proposed boot mappings 
            // are the same: if not, they need saving

            byte[] map = RegistryProvider.GetScancodeMapFromRegistry(MapLocation.KeyMapperVistaMappingsCache);

            if (map == null)
                return false;

            Collection<KeyMapping> maps = GetMappingsFromScancodeMap(map, MappingType.Boot);

            if (maps.Count != _bootMappings.Count)
                return true;

            foreach (KeyMapping km in maps)
            {
                if (_bootMappings.Contains(km) == false)
                    return true;
            }

            return false;
        }

        public static bool IsMapped(KeyMapping map, MappingFilter filter)
        {
            Collection<KeyMapping> maps;

            switch (filter)
            {
                case MappingFilter.All:
                    maps = _allMappings;
                    break;
                case MappingFilter.Boot:
                    maps = _bootMappings;
                    break;
                case MappingFilter.Current:
                    maps = currentFilteredMappings;
                    break;
                case MappingFilter.User:
                    maps = _userMappings;
                    break;
                case MappingFilter.ClearedUser:
                    maps = clearedUserMappings;
                    break;
                case MappingFilter.ClearedBoot:
                    maps = clearedBootMappings;
                    break;
                default:
                    maps = new Collection<KeyMapping>();
                    break;
            }

            return maps.Contains(map);
        }

        public static bool IsMappingPending(KeyMapping map)
        {
            return IsMappingPending(map, Filter);
        }

        public static bool IsMappingPending(KeyMapping map, MappingFilter filter)
        {
            if (_unsavedMappings.Contains(map))
                return false;

            // Did this mapping exist at boot or logon time?
            switch (filter)
            {
                case MappingFilter.ClearedUser:
                case MappingFilter.ClearedBoot:
                    return true; // A cleared mapping is by definition pending
                case MappingFilter.All:
                    return !(_savedBootMappings.Contains(map) | _savedUserMappings.Contains(map));
                case MappingFilter.Boot:
                    return !(_savedBootMappings.Contains(map));
                case MappingFilter.User:
                    return !(_savedUserMappings.Contains(map));
            }

            return true;
        }

        public static KeyMapping GetKeyMapping(int scancode, int extended)
        {
            foreach (KeyMapping mapping in currentFilteredMappings)
            {
                if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
                    return mapping;
            }

            return GetEmptyMapping(new Key(scancode, extended));
        }

        public static KeyMapping GetClearedMapping(int scancode, int extended)
        {
            return GetClearedMapping(scancode, extended, Filter);
        }

        public static KeyMapping GetClearedMapping(int scancode, int extended, MappingFilter filter)
        {
            // If all mappings, look at boot mappings first 
            // in case the key was mapped in both. 

            if (filter == MappingFilter.All || filter == MappingFilter.Boot)
            {
                foreach (KeyMapping mapping in clearedBootMappings)
                {
                    if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
                        return mapping;
                }
            }

            if (filter == MappingFilter.All || filter == MappingFilter.User)
                foreach (KeyMapping mapping in clearedUserMappings)
                {
                    if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
                        return mapping;
                }

            return GetEmptyMapping(new Key(scancode, extended));
        }

        public static void SetFilter(MappingFilter filter)
        {
            // This is a bit problematic as this causes a change which can't be undone 
            // without setting the filter back. Well, problematic unless:
            // a) Implement event codes (or simply "Undo set filter" 
            // but would need to know what previous filters were each time.)
            // or b) .. just clear the stacks.

            if (filter != MappingFilter.All && filter != MappingFilter.Boot && filter != MappingFilter.User)
                throw new ArgumentException("Mappings Filter is not valid");

            undostack.Clear();
            redostack.Clear();

            Filter = filter;
            RaiseMappingsChangedEvent();
        }

        public static KeyMapping GetEmptyMapping(Key from)
        {
            return new KeyMapping(from, new Key(-1, -1));
        }

        public static bool IsEmptyMapping(KeyMapping map)
        {
            return (map.To.Scancode == -1 && map.To.Extended == -1);
        }

        public static bool IsDisabledMapping(KeyMapping map)
        {
            return (map.To.Scancode == 0 && map.To.Extended == 0);
        }

        public static void SaveBootMappingsVista()
        {
            // Well, we need to write to HKLM under Vista.
            // Create a registry file and run it, user will have to allow regedit to run.

            if (
                AppController.ConfirmWriteToProtectedSectionOfRegistryOnVistaOrLater("the changes to your boot mappings") ==
                false)
                return;

            string tempfile = ExportMappingsAsRegistryFile(MappingFilter.Boot, true);

            AppController.WriteRegistryFileVista(tempfile);
        }

        public static void SaveBootMappingsToKeyMapperKey()
        {
            SaveMappings(Mappings.CurrentBootMappings,
                         MapLocation.KeyMapperLocalMachineKeyboardLayout);
            // As have overwritten our stored value with a new one, reload it ...
            GetMappingsFromRegistry(MapLocation.KeyMapperLocalMachineKeyboardLayout);
            // ... and recalculate mappings.
            PopulateMappingLists();
        }

        public static void SaveUserMappingsToKeyMapperKey()
        {
            SaveUserMappingsToKeyMapperKey(false);
        }


        public static void SaveUserMappingsToKeyMapperKey(bool raiseEvent)
        {
            SaveMappings(Mappings.CurrentUserMappings,
                         MapLocation.KeyMapperCurrentUserKeyboardLayout);
            GetMappingsFromRegistry(MapLocation.KeyMapperCurrentUserKeyboardLayout);
            PopulateMappingLists();
            if (raiseEvent)
                RaiseMappingsChangedEvent();
        }

        private static void SaveMappings()
        {
            SaveMappings(Mappings.CurrentUserMappings, MapLocation.CurrentUserKeyboardLayout);
            if (AppController.UserCanWriteBootMappings)
                SaveMappings(Mappings.CurrentBootMappings, MapLocation.LocalMachineKeyboardLayout);
        }

        public static void SaveMappings(Mappings whichMappings, MapLocation whereToSave)
        {
            Collection<KeyMapping> maps;

            switch (whichMappings)
            {
                case Mappings.CurrentBootMappings:
                    maps = _bootMappings;
                    break;
                case Mappings.CurrentUserMappings:
                    maps = _userMappings;
                    break;
                case Mappings.SavedBootMappings:
                    maps = _savedBootMappings;
                    break;
                case Mappings.SavedUserMappings:
                    maps = _savedUserMappings;
                    break;
                default:
                    return;
            }

            RegistryHive hive = 0;
            string keyname = "", valuename = "";

            RegistryProvider.GetRegistryLocation(whereToSave, ref hive, ref keyname, ref valuename);

            RegistryKey registry = null;

            // Need write access so prepare to fail 
            try
            {
                if (hive == RegistryHive.LocalMachine)
                {
                    registry = Registry.LocalMachine.OpenSubKey(keyname, true);
                }
                else if (hive == RegistryHive.CurrentUser)
                {
                    registry = Registry.CurrentUser.OpenSubKey(keyname, true);
                }
            }
            catch (SecurityException ex)
            {
                if (hive == RegistryHive.CurrentUser)
                {
                    // Would expect to be able to write to HKCU
                    Console.WriteLine("Unexpected failure {2} opening {0} on {1} for write access",
                                      keyname, Enum.GetNames(typeof (Mappings))[(int) whichMappings], ex.Message);
                }

                return;
            }

            if (registry == null)
            {
                // Key doesn't exist
                return;
            }

            if (maps.Count == 0)
            {
                registry.SetValue(valuename, new byte[0]);
            }
            else
            {
                registry.SetValue(valuename, GetMappingsAsByteArray(maps));
            }
        }

        private static byte[] GetMappingsAsByteArray(Collection<KeyMapping> maps)
        {
            // Turn mappings into a byte[] 
            int count = maps.Count;
            int size = (16 + (count*4));

            // Check they are all zero.

            var bytemappings = new byte[size];

            // Allow for the null mapping at the end
            bytemappings[8] = (byte) (count + 1);

            const int start = 12;

            for (int i = 0; i < count; i++)
            {
                // Make sure we don't extend beyond array bounds
                if (size <= (start + (i*4) + 3))
                {
                    break;
                }

                KeyMapping map = maps[i];

                // First pair is the action - what the mapped key does.
                bytemappings[start + (i*4)] = (byte) map.To.Scancode;
                bytemappings[start + (i*4) + 1] = (byte) map.To.Extended;

                // Second pair is the physical key which performs the new action
                bytemappings[start + (i*4) + 2] = (byte) map.From.Scancode;
                bytemappings[start + (i*4) + 3] = (byte) map.From.Extended;
            }

            return bytemappings;
        }

        public static string ExportMappingsAsRegistryFile(MappingFilter filter, bool useTempFile)
        {
            // This is the required format:

            // Windows Registry Editor Version 5.00

            // [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]
            // "Scancode Map"=hex:00,00,00,00,00,00,00,00,02,00,00,00,2a,00,3a,00,00,00,00,00

            // [HKEY_CURRENT_USER\Keyboard Layout]
            // "Scancode Map"=hex:00,00,00,00,00,00,00,00,04,00,00,00,5d,e0,1c,e0,1d,00,5b,e0,2a,00,3a,00,00,00,00,00

            // Where there are no mappings, delete the value:
            // "Scancode Map"=-

            string filename;

            if (useTempFile)
                filename = Path.GetTempPath() + Path.GetRandomFileName() + ".reg";
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
                                 FileName = "Key Mappings"
                             };


                if (AppController.DotNetFramework2ServicePackInstalled)
                {
                    AppController.EnableVisualUpgrade(fd);
                }

                DialogResult dr = fd.ShowDialog();

                if (dr != DialogResult.OK)
                {
                    return String.Empty;
                }

                filename = fd.FileName;
            }

            int bootMappingCount = GetMappingCount(MappingFilter.Boot);

            using (var sw = new StreamWriter(filename, false, Encoding.Unicode))
            {
                sw.WriteLine("Windows Registry Editor Version 5.00");
                sw.WriteLine();

                if (filter == MappingFilter.Boot || filter == MappingFilter.All)
                {
                    sw.WriteLine(@"[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]");
                    sw.Write("\"Scancode Map\"=");
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

                    if (bootMappingCount > 0)
                        sw.WriteLine();
                }

                if (filter == MappingFilter.User || filter == MappingFilter.All)
                {
                    sw.WriteLine(@"[HKEY_CURRENT_USER\Keyboard Layout]");

                    sw.Write("\"Scancode Map\"=");

                    if (GetMappingCount(MappingFilter.User) > 0)
                    {
                        sw.Write("hex:");
                        WriteMappingsToStream(sw, GetMappingsAsByteArray(GetMappings(MappingFilter.User)));
                    }
                    else
                        sw.Write("-");

                    sw.WriteLine();
                }
            }

            return filename;
        }

        private static void WriteMappingsToStream(StreamWriter sw, byte[] bytemappings)
        {
            for (int i = 0; i < bytemappings.GetLength(0); i++)
            {
                sw.Write(bytemappings[i].ToString("X", CultureInfo.InvariantCulture).PadLeft(2, (char) 48));
                if (i < bytemappings.GetLength(0) - 1)
                    sw.Write(",");
            }
        }

        #endregion

        #region Mappings Add/Edit/Delete/Undo/Redo

        private static void RaiseMappingsChangedEvent()
        {
            PopulateMappingLists();
            SaveMappings();
            if (MappingsChanged != null)
                MappingsChanged(null, null);
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

            int scancode = map.From.Scancode;
            int extended = map.From.Extended;

            // If user is remapping Left Ctrl, Left Alt, or Delete then s/he must confirm
            // that it could be goodbye to CTRL-ALT-DEL

            if ((scancode == 29 && extended == 0) || (scancode == 56 && extended == 0) ||
                (scancode == 83 && extended == 224))
            {
                string action = IsDisabledMapping(map) ? "disable " : "remap ";

                string warning = "You are attempting to " + action + map.From.Name +
                                 " which is required for CTRL-ALT-DELETE." + (char) 13 +
                                 "If you continue you may not be able to log on" +
                                 " to your PC.";

                string question = "Are you really sure you want to " + action + "this key?";

                if (operatingSystemCapability.ImplementsTaskDialog)
                {
                    TaskDialogResult dr = FormsManager.ShowTaskDialog(question, warning, "Key Mapper",
                                                                      TaskDialogButtons.Yes | TaskDialogButtons.No,
                                                                      TaskDialogIcon.Question);
                    if (dr != TaskDialogResult.Yes)
                        return false;
                }
                else
                {
                    DialogResult dr = MessageBox.Show(warning + (char) 13 + (char) 13 + question, "Key Mapper",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 0);

                    if (dr != DialogResult.Yes)
                        return false;
                }
            }

            // If user is remapping Pause, then suggest they will want to disable Num Lock as well.

            bool disableNumLock = false;

            if (scancode == 29 && extended == 225 && IsDisabledMapping(map) == false)
            {
                // Is Num Lock already disabled or remapped?
                bool numLockIsDisabled = false;
                bool numLockIsMapped = false;

                foreach (KeyMapping km in _bootMappings)
                    if (km.From.Scancode == 69)
                    {
                        if (IsDisabledMapping(km))
                            numLockIsDisabled = true;
                        else
                            numLockIsMapped = true;
                    }

                // Num Lock could be disabled or mapped in boot
                // but overridden in user mappings.
                foreach (KeyMapping km in _userMappings)
                {
                    if (km.From.Scancode == 69)
                    {
                        if (IsDisabledMapping(km))
                            numLockIsDisabled = true;
                        else
                            numLockIsMapped = true;
                    }
                }

                if (numLockIsDisabled == false)
                {
                    string warning = "If you remap Pause, the Num Lock key will be disabled" +
                                     (numLockIsMapped
                                          ? ((char) 13 + "and your existing Num Lock mapping will be removed.")
                                          : ".");

                    const string question = "Do you still want to remap Pause?";

                    if (operatingSystemCapability.ImplementsTaskDialog)
                    {
                        TaskDialogResult dr = FormsManager.ShowTaskDialog(question, warning, "Key Mapper",
                                                                          TaskDialogButtons.Yes | TaskDialogButtons.No,
                                                                          TaskDialogIcon.Question);
                        if (dr != TaskDialogResult.Yes)
                            return false;
                    }
                    else
                    {
                        DialogResult dr = MessageBox.Show(warning + (char) 13 + (char) 13 + question, "Key Mapper",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 0);

                        if (dr != DialogResult.Yes)
                            return false;
                    }
                    disableNumLock = true;
                }
            }

            if (noStackNoEventRaised == false)
                PushMappingsOntoUndoStack();

            // Check for any existing mappings for this key
            // if they exist, this mapping needs to replace them.

            if (Filter == MappingFilter.Boot)
            {
                map.SetType(MappingType.Boot);

                KeyMapping existingMap = GetKeyMapping(map.From.Scancode, map.From.Extended);
                if (existingMap.IsEmpty() == false && existingMap.MapType == MappingType.Boot)
                    _bootMappings.Remove(existingMap);

                _bootMappings.Add(map);
            }
            else
            {
                map.SetType(MappingType.User);

                KeyMapping existingMap = GetKeyMapping(map.From.Scancode, map.From.Extended);
                if (existingMap.IsEmpty() == false && existingMap.MapType == MappingType.User)
                    _userMappings.Remove(existingMap);

                _userMappings.Add(map);
            }

            if (disableNumLock)
            {
                var nl = new KeyMapping(new Key(69, 0), new Key(0, 0));
                AddMapping(nl, true);
            }

            if (noStackNoEventRaised == false)
                RaiseMappingsChangedEvent();

            return true;
        }

        public static void DeleteMapping(KeyMapping map, MappingFilter filter)
        {
            if (!map.IsValid())
                throw new ArgumentException("Can't delete an invalid map");

            PushMappingsOntoUndoStack();

            switch (filter)
            {
                case MappingFilter.All:
                    // Could be mapped in both HKCU and HKLM so 
                    // remove the HKCU mapping first, but not both..

                    if (_userMappings.Contains(map))
                    {
                        _userMappings.Remove(map);
                    }
                    else if (_bootMappings.Contains(map))
                    {
                        _bootMappings.Remove(map);
                    }
                    break;

                case MappingFilter.Boot:
                    if (_bootMappings.Contains(map))
                    {
                        _bootMappings.Remove(map);
                    }
                    break;

                case MappingFilter.User:
                    if (_userMappings.Contains(map))
                    {
                        _userMappings.Remove(map);
                    }
                    break;
                default:
                    break;
            }

            RaiseMappingsChangedEvent();
        }

        public static void DeleteMapping(KeyMapping map)
        {
            DeleteMapping(map, Filter);
        }

        public static void ClearMappings()
        {
            PushMappingsOntoUndoStack();

            if (Filter != MappingFilter.User)
                _bootMappings = new Collection<KeyMapping>();

            if (Filter != MappingFilter.Boot)
                _userMappings = new Collection<KeyMapping>();

            RaiseMappingsChangedEvent();
        }

        public static void RevertToStartupMappings()
        {
            PushMappingsOntoUndoStack();
            _bootMappings = CopyMappings(_savedBootMappings);
            _userMappings = CopyMappings(_savedUserMappings);
            RaiseMappingsChangedEvent();
        }

        public static void UndoMappingChange()
        {
            if (undostack.Count < 1)
                return;

            PushMappingsOntoRedoStack();
            PopMappingsOffUndoStack();
            RaiseMappingsChangedEvent();
        }

        public static void RedoMappingChange()
        {
            if (redostack.Count < 1)
                return;

            // To 'redo', the latest entry on the redo stack is popped into current members
            // which is itself popped onto the undo stack.
            PushMappingsOntoUndoStack();
            PopMappingsOffRedoStack();
            RaiseMappingsChangedEvent();
        }

        #endregion

        #region Registry methods

        private static Collection<KeyMapping> GetMappingsFromScancodeMap(byte[] map, MappingType type)
        {
            // Transform the byte array into keymappings

            var maps = new Collection<KeyMapping>();

            int count = 0;
            int length = map.GetLength(0);

            // How many mappings are there?
            // (Make sure there are at least 8 bytes in the array)

            if (length > 8)
                count = map[8] - 1;

            if (count == 0)
            {
                return maps;
            }

            const int start = 12;

            for (int i = 0; i < count; i++)
            {
                // Make sure we don't extend beyond array bounds
                if (length >= (start + (i*4) + 3))
                {
                    // First pair is the action - what the mapped key does.
                    int word1 = map[start + (i*4)];
                    int word2 = map[start + (i*4) + 1];

                    var tokey = new Key(word1, word2);

                    // Second pair is the physical key which performs the new action
                    word1 = map[start + (i*4) + 2];
                    word2 = map[start + (i*4) + 3];

                    var fromkey = new Key(word1, word2);

                    var mapping = new KeyMapping(fromkey, tokey);

                    if (mapping.IsValid())
                    {
                        mapping.SetType(type);
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
            GetMappingsFromRegistry(MapLocation.CurrentUserKeyboardLayout);
            GetMappingsFromRegistry(MapLocation.KeyMapperLocalMachineKeyboardLayout);
            GetMappingsFromRegistry(MapLocation.KeyMapperCurrentUserKeyboardLayout);
            PopulateMappingLists();
        }

        private static void GetMappingsFromRegistry(MapLocation location)
        {
            var mappings = new Collection<KeyMapping>();

            MappingType type = MappingType.Null;
            switch (location)
            {
                case MapLocation.LocalMachineKeyboardLayout:
                case MapLocation.KeyMapperLocalMachineKeyboardLayout:
                    type = MappingType.Boot;
                    break;
                case MapLocation.CurrentUserKeyboardLayout:
                case MapLocation.KeyMapperCurrentUserKeyboardLayout:
                    type = MappingType.User;
                    break;
            }


            byte[] map = RegistryProvider.GetScancodeMapFromRegistry(location);

            if (map != null)
                mappings = GetMappingsFromScancodeMap(map, type);

            switch (location)
            {
                case MapLocation.LocalMachineKeyboardLayout:
                    _bootMappings = mappings;
                    break;
                case MapLocation.CurrentUserKeyboardLayout:
                    _userMappings = mappings;
                    break;
                case MapLocation.KeyMapperLocalMachineKeyboardLayout:
                    _savedBootMappings = mappings;
                    break;
                case MapLocation.KeyMapperCurrentUserKeyboardLayout:
                    _savedUserMappings = mappings;
                    break;
                default:
                    break;
            }
        }

        #endregion

        public static event EventHandler<EventArgs> MappingsChanged;

        private static void PushMappingsOntoUndoStack()
        {
            undostack.Push(CopyMappings(_userMappings), CopyMappings(_bootMappings));
        }

        private static void PushMappingsOntoRedoStack()
        {
            redostack.Push(CopyMappings(_userMappings), CopyMappings(_bootMappings));
        }

        private static void PopMappingsOffUndoStack()
        {
            _bootMappings = undostack.BootStack.Pop();
            _userMappings = undostack.UserStack.Pop();
        }

        private static void PopMappingsOffRedoStack()
        {
            _bootMappings = redostack.BootStack.Pop();
            _userMappings = redostack.UserStack.Pop();
        }
    }
}