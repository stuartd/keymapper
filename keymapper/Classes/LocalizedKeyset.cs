using System.Collections;
using System.Collections.Generic;

namespace KeyMapper
{

	/// <summary>
	///  This represents all the keyboard keys in the vurrent keyboard layout.
	/// </summary>
	public class LocalizedKeySet
	{
		#region Fields

		private List<int> _localizableKeys = new KeyDataXml().LocalizableKeys;
		private List<int> _nonLocalizableKeys = new KeyDataXml().NonLocalizableKeys;

		private Hashtable _localizableKeyNames = new Hashtable();
		private Hashtable _nonLocalizableKeyNames = new Hashtable();

		private Hashtable _keys = new Hashtable();

		private List<int> _overlongkeys = new List<int>(0);

		#endregion

		#region Constructor and other public methods

		public LocalizedKeySet()
		{
			_keys.Clear();
			_overlongkeys.Clear();

			GetLocalizableKeyNames();
			GetNonLocalizableKeyNames();

			foreach (DictionaryEntry de in _localizableKeyNames)
				_keys.Add(de.Key, de.Value);
			
			foreach (DictionaryEntry de in _nonLocalizableKeyNames)
				_keys.Add(de.Key, de.Value);

		}

		public bool ContainsKey(int hash)
		{
			return _keys.Contains(hash);
		}

		public string GetKeyName(int hash)
		{
			return (string)_keys[hash];
		}

		public bool IsKeyNameOverlong(int hash)
		{
			return _overlongkeys.Contains(hash);
		}

		public bool IsKeyLocalizable(int hash)
		{
			return _localizableKeys.Contains(hash);
		}


		#endregion

		#region Other methods

		private void GetNonLocalizableKeyNames()
		{
			// These have to be extracted from the keycode XML
			// as thay aren't available otherwise (they don't change)

			KeyDataXml kd = new KeyDataXml();

			foreach (int code in _nonLocalizableKeys)
			{
				_nonLocalizableKeyNames.Add(code, kd.GetKeyNameFromCode(code));
			}

		}

		private void GetLocalizableKeyNames()
		{
			_localizableKeyNames.Clear();

			foreach (int hash in _localizableKeys)
			{
				// None of the localizable names need the extended bit.
				int scancode = AppController.GetScancodeFromHash(hash);

				// Need to track if a localizable key is a symbol - shifter-symbol
				// combination but it's length is not 3 - i.e. instead of 1 and ! 
				// it is Qaf and RehYehAlefLam (as on the Farsi keyboard)

				bool overlong = false;
				string name = KeyboardHelper.GetKeyName(scancode, ref overlong);

				_nonLocalizableKeyNames.Add(hash, name);
				if (overlong)
				{
					// Console.WriteLine("Adding overlong key: code {0} name length: {1} name: {2}", hash, name.Length, name);
					_overlongkeys.Add(hash);
				}
			}

		}

		#endregion
	}
}
