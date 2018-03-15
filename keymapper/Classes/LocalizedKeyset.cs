using System.Collections;
using System.Collections.Generic;

namespace KeyMapper.Classes
{
    /// <summary>
    ///  This represents all the keyboard keys in the vurrent keyboard layout.
    /// </summary>
    public class LocalizedKeySet
    {
        private readonly Hashtable keys = new Hashtable();
        private readonly Hashtable localizableKeyNames = new Hashtable();
        private readonly IList<int> localizableKeys = new KeyDataXml().LocalizableKeys;
        private readonly Hashtable nonLocalizableKeyNames = new Hashtable();
        private readonly IList<int> nonLocalizableKeys = new KeyDataXml().NonLocalizableKeys;

        private readonly List<int> overlongkeys = new List<int>(0);

        public LocalizedKeySet()
        {
            keys.Clear();
            overlongkeys.Clear();

            GetLocalizableKeyNames();
            GetNonLocalizableKeyNames();

            foreach (DictionaryEntry de in localizableKeyNames) {
				keys.Add(de.Key, de.Value);
			}

			foreach (DictionaryEntry de in nonLocalizableKeyNames) {
				keys.Add(de.Key, de.Value);
			}
		}

        public bool ContainsKey(int hash)
        {
            return keys.Contains(hash);
        }

        public string GetKeyName(int hash)
        {
            return (string) keys[hash];
        }

        public bool IsKeyNameOverlong(int hash)
        {
            return overlongkeys.Contains(hash);
        }

        public bool IsKeyLocalizable(int hash)
        {
            return localizableKeys.Contains(hash);
        }

        private void GetNonLocalizableKeyNames()
        {
            // These have to be extracted from the keycode XML
            // as thay aren't available otherwise (they don't change)

            var kd = new KeyDataXml();

            foreach (int code in nonLocalizableKeys)
            {
                nonLocalizableKeyNames.Add(code, kd.GetKeyNameFromCode(code));
            }
        }

        private void GetLocalizableKeyNames()
        {
            localizableKeyNames.Clear();

            foreach (int hash in localizableKeys)
            {
                // None of the localizable names need the extended bit.
                int scancode = KeyHasher.GetScancodeFromHash(hash);

                // Need to track if a localizable key is a symbol - shifter-symbol
                // combination but it's length is not 3 - i.e. instead of 1 and ! 
                // it is Qaf and RehYehAlefLam (as on the Farsi keyboard)

                bool overlong = false;
                string name = KeyboardHelper.GetKeyName(scancode, ref overlong);

                nonLocalizableKeyNames.Add(hash, name);
                if (overlong)
                {
                    // Console.WriteLine("Adding overlong key: code {0} name length: {1} name: {2}", hash, name.Length, name);
                    overlongkeys.Add(hash);
                }
            }
        }
    }
}