using System.Collections;
using System.Collections.Generic;

namespace KeyMapper.Classes
{
    /// <summary>
    ///  This represents all the keyboard keys in the current keyboard layout.
    /// </summary>
    public class LocalizedKeySet
    {
        private readonly Hashtable _keys = new Hashtable();
        private readonly Hashtable _localizableKeyNames = new Hashtable();
        private readonly IList<int> _localizableKeys = new KeyDataXml().LocalizableKeys;
        private readonly Hashtable _nonLocalizableKeyNames = new Hashtable();
        private readonly IList<int> _nonLocalizableKeys = new KeyDataXml().NonLocalizableKeys;

        private readonly List<int> _overlongkeys = new List<int>(0);

        public LocalizedKeySet()
        {
            _keys.Clear();
            _overlongkeys.Clear();

            GetLocalizableKeyNames();
            GetNonLocalizableKeyNames();

            foreach (DictionaryEntry de in _localizableKeyNames) {
				_keys.Add(de.Key, de.Value);
			}

			foreach (DictionaryEntry de in _nonLocalizableKeyNames) {
				_keys.Add(de.Key, de.Value);
			}
		}

        public bool ContainsKey(int hash)
        {
            return _keys.Contains(hash);
        }

        public string GetKeyName(int hash)
        {
            return (string) _keys[hash];
        }

        public bool IsKeyNameOverlong(int hash)
        {
            return _overlongkeys.Contains(hash);
        }

        public bool IsKeyLocalizable(int hash)
        {
            return _localizableKeys.Contains(hash);
        }

        private void GetNonLocalizableKeyNames()
        {
            // These have to be extracted from the keycode XML
            // as they aren't available otherwise (they don't change)

            var kd = new KeyDataXml();

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
                int scanCode = KeyHasher.GetScanCodeFromHash(hash);

                // Need to track if a localizable key is a symbol - shifter-symbol
                // combination but it's length is not 3 - i.e. instead of 1 and ! 
                // it is Qaf and RehYehAlefLam (as on the Farsi keyboard)

                bool overlong = false;
                string name = KeyboardHelper.GetKeyName(scanCode, ref overlong);

                _nonLocalizableKeyNames.Add(hash, name);
                if (overlong)
                {
                    // Console.WriteLine("Adding overlong key: code {0} name length: {1} name: {2}", hash, name.Length, name);
                    _overlongkeys.Add(hash);
                }
            }
        }
    }
}