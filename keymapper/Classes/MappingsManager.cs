using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace KeyMapper
{
	public static class MappingsManager
	{
		public static event EventHandler<EventArgs> MappingsChanged;

		#region Fields
		// Undo/Redo stacks are implemented as pairs.
		private static UndoRedoMappingStack _undostack = new UndoRedoMappingStack();
		private static UndoRedoMappingStack _redostack = new UndoRedoMappingStack();

		// Saved mapping data
		private static Collection<KeyMapping> _savedBootMappings = new Collection<KeyMapping>();
		private static Collection<KeyMapping> _savedUserMappings = new Collection<KeyMapping>();

		// Mapping data
		private static Collection<KeyMapping> _bootMappings = new Collection<KeyMapping>();
		private static Collection<KeyMapping> _userMappings = new Collection<KeyMapping>();
		private static Collection<KeyMapping> _allMappings = new Collection<KeyMapping>();

		// Current mappings based on current filter
		private static Collection<KeyMapping> _currentFilteredMappings = new Collection<KeyMapping>();

		// Need to maintain two collections of mappings which have been cleared
		// (ie which existed at boot or logon but don't exist now)
		private static Collection<KeyMapping> _clearedUserMappings = new Collection<KeyMapping>();
		private static Collection<KeyMapping> _clearedBootMappings = new Collection<KeyMapping>();

		private static MappingFilter _filter = MappingFilter.All;

		#endregion

		#region Properties

		public static MappingFilter Filter
		{
			get
			{
				return _filter;
			}
		}

		public static int UndoStackCount
		{
			get { return _undostack.Count; }
		}

		public static int RedoStackCount
		{
			get { return _redostack.Count; }
		}

		public static Collection<KeyMapping> ClearedMappings
		{
			get
			{
				switch (_filter)
				{
					case MappingFilter.All:
						Collection<KeyMapping> temp = CopyMappings(_clearedBootMappings);
						foreach (KeyMapping km in _clearedUserMappings)
						{
							temp.Add(km);
						}
						return temp;

					case MappingFilter.Boot:
						return _clearedBootMappings;

					case MappingFilter.User:
						return _clearedUserMappings;

					default:
						return new Collection<KeyMapping>();
				}
			}
		}

		#endregion

		#region Private stack methods

		private static void PushMappingsOntoUndoStack()
		{
			_undostack.Push(CopyMappings(_userMappings), CopyMappings(_bootMappings));
		}

		private static void PushMappingsOntoRedoStack()
		{
			_redostack.Push(CopyMappings(_userMappings), CopyMappings(_bootMappings));
		}

		private static void PopMappingsOffUndoStack()
		{
			_bootMappings = _undostack.BootStack.Pop();
			_userMappings = _undostack.UserStack.Pop();
		}

		private static void PopMappingsOffRedoStack()
		{
			_bootMappings = _redostack.BootStack.Pop();
			_userMappings = _redostack.UserStack.Pop();
		}

		#endregion

		#region mapping utility methods

		public static void PopulateMappingLists()
		{
			// Populate the internal mapping lists at startup and when mappings change.
			_currentFilteredMappings.Clear();
			_clearedUserMappings.Clear();
			_clearedBootMappings.Clear();
			_allMappings.Clear();


			foreach (KeyMapping bootmap in _bootMappings)
			{
				// Add everything to All Mappings
				_allMappings.Add(bootmap);

				bool _addToFilteredMappings = true;

				switch (_filter)
				{
					case MappingFilter.All:
						// If filter is All then need to check if this boot mapping is overriden by a user mapping 
						// Because if it is, we don't add it.

						foreach (KeyMapping usermap in _userMappings)
						{
							if (bootmap.From == usermap.From)
							{
								_addToFilteredMappings = false;
								break;
							}
						}

						break;

					case MappingFilter.User:
						_addToFilteredMappings = false;
						break;

				}

				if (_addToFilteredMappings)
					_currentFilteredMappings.Add(bootmap);

			}

			foreach (KeyMapping usermap in _userMappings)
			{
				_allMappings.Add(usermap);

				if (_filter == MappingFilter.All || _filter == MappingFilter.User)
					_currentFilteredMappings.Add(usermap);
			}

			// Finally, skip through the user and boot mappings so we can populate the cleared mappings lists
			foreach (KeyMapping map in _savedUserMappings)
			{
				if (_userMappings.Contains(map) == false)
					_clearedUserMappings.Add(map);
			}

			foreach (KeyMapping map in _savedBootMappings)
			{
				if (_bootMappings.Contains(map) == false)
					_clearedBootMappings.Add(map);
			}
		}


		private static Collection<KeyMapping> CopyMappings(Collection<KeyMapping> mappings)
		{
			Collection<KeyMapping> copy = new Collection<KeyMapping>();

			foreach (KeyMapping map in mappings)
			{
				if (map.IsValid())
					copy.Add(map);
				else
					throw new ArgumentException("Can't copy an invalid map");
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
					return _currentFilteredMappings;

				case MappingFilter.Boot:
					return _bootMappings;

				case MappingFilter.User:
					return _userMappings;

				case MappingFilter.ClearedUser:
					return _clearedUserMappings;

				case MappingFilter.ClearedBoot:
					return _clearedBootMappings;
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
					return _currentFilteredMappings.Count;

				case MappingFilter.Boot:
					return _bootMappings.Count;

				case MappingFilter.User:
					return _userMappings.Count;

				case MappingFilter.ClearedUser:
					return _clearedUserMappings.Count;

				case MappingFilter.ClearedBoot:
					return _clearedBootMappings.Count;

				default:
					return 0;
			}

		}

		public static bool IsRestartRequired()
		{
			if (_clearedBootMappings.Count != 0 || (_savedBootMappings.Count != _bootMappings.Count))
				return true;

			// Need to iterate through to see if any have changed.
			foreach (KeyMapping km in _bootMappings)
			{
				if (_savedBootMappings.Contains(km) == false)
					return true;
			}

			return false;
		}

		public static bool IsLogOnRequired()
		{
			if (_clearedUserMappings.Count != 0 || (_savedUserMappings.Count != _userMappings.Count))
				return true;

			// Need to look for new mappings as well.
			foreach (KeyMapping km in _userMappings)
			{
				if (_savedUserMappings.Contains(km) == false)
					return true;
			}

			return false;
		}

		public static bool IsMapped(KeyMapping map)
		{
			return IsMapped(map, _filter);
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
					maps = _currentFilteredMappings;
					break;
				case MappingFilter.User:
					maps = _userMappings;
					break;
				case MappingFilter.ClearedUser:
					maps = _clearedUserMappings;
					break;
				case MappingFilter.ClearedBoot:
					maps = _clearedBootMappings;
					break;
				default:
					maps = new Collection<KeyMapping>();
					break;
			}

			return maps.Contains(map);

		}

		public static bool IsMappingPending(KeyMapping map)
		{
			return IsMappingPending(map, _filter);
		}

		public static bool IsMappingPending(KeyMapping map, MappingFilter filter)
		{

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
			foreach (KeyMapping mapping in _currentFilteredMappings)
			{
				if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
					return mapping;
			}

			return MappingsManager.GetEmptyMapping(new Key(scancode, extended));
		}

		public static KeyMapping GetClearedMapping(int scancode, int extended)
		{
			return GetClearedMapping(scancode, extended, _filter);
		}

		public static KeyMapping GetClearedMapping(int scancode, int extended, MappingFilter filter)
		{
			// If all mappings, look at boot mappings first 
			// in case the key was mapped in both. 

			if (filter == MappingFilter.All || filter == MappingFilter.Boot)
			{
				foreach (KeyMapping mapping in _clearedBootMappings)
				{
					if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
						return mapping;
				}
			}

			if (filter == MappingFilter.All || filter == MappingFilter.User)
				foreach (KeyMapping mapping in _clearedUserMappings)
				{
					if (mapping.From.Scancode == scancode && mapping.From.Extended == extended)
						return mapping;
				}

			return MappingsManager.GetEmptyMapping(new Key(scancode, extended));
		}

		public static bool WasClearedMappingUserMapping(KeyMapping map)
		{
			// Was this cleared mapping from user or boot?
			return _savedUserMappings.Contains(map);
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

			_undostack.Clear();
			_redostack.Clear();

			_filter = filter;
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

		public static void SaveMappings()
		{
			SaveMappings(Mappings.CurrentUserMappings, MapLocation.CurrentUserKeyboardLayout);
			if (AppController.UserCanWriteBootMappings)
				SaveMappings(Mappings.CurrentBootMappings, MapLocation.LocalMachineKeyboardLayout);
		}

		public static void SaveMappings(Mappings col, MapLocation where)
		{

			Collection<KeyMapping> maps;

			switch (col)
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

			// System.Windows.Forms.MessageBox.Show("Saving " + maps.Count.ToString() + " mappings");

			RegistryHive hive = 0;
			string keyname = "", valuename = "";

			GetRegistryLocation(where, ref hive, ref keyname, ref valuename);

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
			catch (System.Security.SecurityException ex)
			{
				if (hive == RegistryHive.CurrentUser)
				{
					// Would expect to be able to write to HKCU
					Console.WriteLine("Unexpected failure {2} opening {0} on {1} for write access",
						keyname, Enum.GetNames(typeof(Mappings))[(int)col], ex.Message);
				}

				return;
			}

			if (registry == null)
			{
				// Key doesn't exist
				return;
			}

			int count = maps.Count;

			if (count == 0)
			{
				// Remove the key.
				registry.DeleteValue(valuename, false);
			}
			else
			{
				// Turn mappings into a byte[] 

				int size = (16 + (count * 4));

				// Check they are all zero.

				byte[] bytemappings = new byte[size];

				// Allow for the null mapping at the end
				bytemappings[8] = (byte)(count + 1);

				int start = 12;

				for (int i = 0; i < count; i++)
				{
					// Make sure we don't extend beyond array bounds
					if (size > (start + (i * 4) + 3))
					{
						KeyMapping map = maps[i];

						// First pair is the action - what the mapped key does.

						int word2 = map.To.Extended;

						bytemappings[start + (i * 4)] = (byte)map.To.Scancode; ;
						bytemappings[start + (i * 4) + 1] = (byte)map.To.Extended;

						// Second pair is the physical key which performs the new action
						bytemappings[start + (i * 4) + 2] = (byte)map.From.Scancode;
						bytemappings[start + (i * 4) + 3] = (byte)map.From.Extended;
					}
					else
						break;
				}

				registry.SetValue(valuename, bytemappings);
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
			if (!map.IsValid())
			{
				return false;
			}

			int scancode = map.From.Scancode;
			int extended = map.From.Extended;

			// If user is remapping Left Ctrl, Left Alt, or Delete then s/he must confirm
			// that it ould be goodbye to CTRL-ALT-DEL

			if ((scancode == 29 && extended == 0) || (scancode == 56 && extended == 0) || (scancode == 83 && extended == 224))
			{
				string action = IsDisabledMapping(map) ? "disable " : "remap ";

				string warning = "You are attempting to " + action + map.From.Name +
					" which is required for CTRL-ALT-DELETE." + (char)13 + "If you continue you may not be able to log on" +
				" to your PC." + (char)13 + (char)13 + "Are you really sure you want to " + action + "this key?";

				DialogResult dr = MessageBox.Show(warning, "KeyMapper", MessageBoxButtons.YesNo,
					MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 0);

				if (dr != DialogResult.Yes)
					return false;
			}


			PushMappingsOntoUndoStack();

			if (_filter == MappingFilter.Boot)
			{
				map.SetType(MappingType.Boot);
				_bootMappings.Add(map);
			}
			else
			{
				map.SetType(MappingType.User);
				_userMappings.Add(map);
			}

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
			DeleteMapping(map, _filter);
		}

		public static void ClearMappings()
		{
			PushMappingsOntoUndoStack();

			if (_filter != MappingFilter.User)
				_bootMappings = new Collection<KeyMapping>();

			if (_filter != MappingFilter.Boot)
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
			if (_undostack.Count < 1)
				return;

			PushMappingsOntoRedoStack();
			PopMappingsOffUndoStack();
			RaiseMappingsChangedEvent();
		}

		public static void RedoMappingChange()
		{

			if (_redostack.Count < 1)
				return;

			// To 'redo', the latest entry on the redo stack is popped into current members
			// which is itself popped onto the undo stack.
			PushMappingsOntoUndoStack();
			PopMappingsOffRedoStack();
			RaiseMappingsChangedEvent();
		}

		#endregion

		#region Registry methods

		private static bool GetRegistryLocation(MapLocation which, ref RegistryHive hive, ref string keyname, ref string valuename)
		{
			hive = RegistryHive.CurrentUser;

			switch (which)
			{
				case MapLocation.LocalMachineKeyboardLayout:
					hive = RegistryHive.LocalMachine;
					keyname = @"SYSTEM\CurrentControlSet\Control\Keyboard Layout";
					valuename = "Scancode Map";
					break;
				case MapLocation.CurrentUserKeyboardLayout:
					keyname = @"Keyboard Layout";
					valuename = "Scancode Map";
					break;
				case MapLocation.KeyMapperLocalMachineKeyboardLayout:
					keyname = AppController.ApplicationRegistryKeyName;
					valuename = "BootMaps";
					break;
				case MapLocation.KeyMapperCurrentUserKeyboardLayout:
					keyname = AppController.ApplicationRegistryKeyName;
					valuename = "UserMaps";
					break;
				default:
					return false;
			}

			return true;
		}

		private static byte[] GetScancodeMapFromRegistry(MapLocation which)
		{

			RegistryKey registry = null;
			byte[] bytecodes = null;
			RegistryHive hive = RegistryHive.CurrentUser;
			string keyname = "", valuename = "";

			if (GetRegistryLocation(which, ref hive, ref keyname, ref valuename))
			{
				if (hive == RegistryHive.LocalMachine)
				{
					registry = Registry.LocalMachine.OpenSubKey(keyname);
				}
				else if (hive == RegistryHive.CurrentUser)
				{
					registry = Registry.CurrentUser.OpenSubKey(keyname);
				}
			}

			if (registry == null)
				return null;

			object keyvalue = registry.GetValue(valuename, null);

			if (keyvalue == null ||
				registry.GetValueKind(valuename) != RegistryValueKind.Binary ||
				keyvalue.GetType() != Type.GetType("System.Byte[]"))
			{
				// Not there, or not the right type.
				return null;
			}

			// Can't see how this cast can fail, shrug, will return null anyway.
			bytecodes = keyvalue as byte[];

			return bytecodes;

		}

		private static Collection<KeyMapping> GetMappingsFromScancodeMap(byte[] map, MappingType type)
		{
			// Transform the byte array into keymappings

			Collection<KeyMapping> maps = new Collection<KeyMapping>();

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

			int start = 12;

			for (int i = 0; i < count; i++)
			{
				// Make sure we don't extend beyond array bounds
				if (length >= (start + (i * 4) + 3))
				{
					// First pair is the action - what the mapped key does.
					int word1 = map[start + (i * 4)];
					int word2 = map[start + (i * 4) + 1];

					Key tokey = new Key(word1, word2);

					// Second pair is the physical key which performs the new action
					word1 = map[start + (i * 4) + 2];
					word2 = map[start + (i * 4) + 3];

					Key fromkey = new Key(word1, word2);

					KeyMapping mapping = new KeyMapping(fromkey, tokey);

					if (mapping.IsValid())
					{
						mapping.SetType(type);
						maps.Add(mapping);
					}
					else
					// Just ignore it and hope it goes away.
					// A manually added - or garbled - entry could be invalid.
					{ }
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

		public static void GetMappingsFromRegistry(MapLocation location)
		{

			Collection<KeyMapping> mappings = new Collection<KeyMapping>();

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


			byte[] map = GetScancodeMapFromRegistry(location);

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

		#region UndoRedoMappingStack Class

		private class UndoRedoMappingStack
		{
			private Stack<Collection<KeyMapping>> _userstack = new Stack<Collection<KeyMapping>>();
			private Stack<Collection<KeyMapping>> _bootstack = new Stack<Collection<KeyMapping>>();

			public Stack<Collection<KeyMapping>> UserStack
			{
				get { return _userstack; }
			}

			public Stack<Collection<KeyMapping>> BootStack
			{
				get { return _bootstack; }
			}

			public void Push(Collection<KeyMapping> usermaps, Collection<KeyMapping> bootmaps)
			{
				_userstack.Push(usermaps);
				_bootstack.Push(bootmaps);
			}

			public int Count
			{
				get
				{
					return _userstack.Count;
				}
			}

			public void Clear()
			{
				_userstack.Clear();
				_bootstack.Clear();
			}
		}

		#endregion

	}


	#region Enums

	public enum MapLocation
	{
		LocalMachineKeyboardLayout, CurrentUserKeyboardLayout, KeyMapperLocalMachineKeyboardLayout, KeyMapperCurrentUserKeyboardLayout
	}

	public enum Mappings
	{
		CurrentBootMappings, CurrentUserMappings, SavedBootMappings, SavedUserMappings
	}

	public enum MappingFilter
	{
		All, Current, Boot, User, ClearedBoot, ClearedUser
	}

	#endregion
}



