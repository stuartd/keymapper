using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Globalization;
using System.IO;

namespace KeyMapper
{

    class KeyDataXml : IKeyData
    {

        /// This class handles extracting the key data from XML files
        /// via XPath.

        #region Fields

        System.Reflection.Assembly _currentassembly = null;
        string _keyfilename = "keycodes.xml";
        string _keyboardfilename = "keyboards.xml";
        XPathNavigator _navigator;
        string _commonlyUsedKeysGroupName = "Commonly Used";
        string _allKeysGroupName = "All Keys";

        #endregion

        #region Constructor
        public KeyDataXml()
        {

            _currentassembly = System.Reflection.Assembly.GetExecutingAssembly();
            // Initialise our navigator from the embedded XML keys file.
            using (Stream xmlstream = GetXMLDocumentAsStream(_keyfilename))
            {
                XPathDocument document = new XPathDocument(xmlstream);
                _navigator = document.CreateNavigator();
            }

        }

        #endregion

        #region XML Methods

        private System.IO.Stream GetXMLDocumentAsStream(string name)
        {
            return _currentassembly.GetManifestResourceStream("KeyMapper.XML." + name);
        }

        private static string GetElementValue(string elementname, XPathNavigator node)
        {
            XPathNodeIterator element = node.SelectChildren(elementname, "");

            if (element.Count > 0)
            {
                element.MoveNext();
                return element.Current.Value;
            }
            return String.Empty;
        }

        #endregion

        #region IKeyData Members

        public KeyboardLayoutType GetKeyboardLayoutType(string locale)
        {
            if (String.IsNullOrEmpty(locale))
                return KeyboardLayoutType.US;

            // Get the layout type - US, European etc. The locale in the XML file must be upper case!
            string expression = @"/keyboards/keyboard[locale='" + locale.ToUpper(CultureInfo.InvariantCulture) + "']";

            XPathNodeIterator iterator;

            using (Stream xmlstream = GetXMLDocumentAsStream(_keyboardfilename))
            {
                XPathDocument document = new XPathDocument(xmlstream);
                XPathNavigator nav = document.CreateNavigator();

                iterator = (XPathNodeIterator)nav.Select(expression);

            }


            int value = 0; // Default to US 

            if (iterator.Count == 1)
            {
                iterator.MoveNext();
                string layout = GetElementValue("layout", iterator.Current);
                if (String.IsNullOrEmpty(layout) == false)
                {
                    value = Int32.Parse(layout, CultureInfo.InvariantCulture.NumberFormat);
                }
            }

            return (KeyboardLayoutType)value;
        }

        public List<string> GetGroupList(int threshold)
        {
            string expression;
            XPathNodeIterator iterator;
            List<string> groups = new List<string>();

            switch (threshold)
            {
                case -1:
                    // Get all the group names: add an extra one at the top with all the keys in.
                    groups.Add(_allKeysGroupName);
                    expression = "/KeycodeData/keycodes/group[not(.=preceding::*/group)] ";
                    iterator = (XPathNodeIterator)_navigator.Select(expression);

                    foreach (XPathNavigator node in iterator)
                    {
                        groups.Add(node.Value);
                    }
                    break;


                case 0:

                    // Get all the groups which have a working member:
                    expression = @"/KeycodeData/keycodes[useful='0']";
                    iterator = (XPathNodeIterator)_navigator.Select(expression);

                    foreach (XPathNavigator node in iterator)
                    {
                        string group = GetElementValue("group", node);
                        if (groups.Contains(group) == false)
                            groups.Add(group);
                    }

                    break;

                case 1:

                    // For this threshold, create an extra group of commonly used keys:
                    // Most of the Media Keys, browser and email, and Print Screen fall into this category.

                    // They have a threshold of 2.

                    groups.Add(_commonlyUsedKeysGroupName);

                    expression = @"/KeycodeData/keycodes[useful='1']";
                    iterator = (XPathNodeIterator)_navigator.Select(expression);

                    foreach (XPathNavigator node in iterator)
                    {
                        string group = GetElementValue("group", node);
                        if (groups.Contains(group) == false)
                            groups.Add(group);
                    }

                    break;


            }

            return groups;
        }

        public List<string> GetSortedGroupList(int threshold)
        {
            List<string> groups = GetGroupList(threshold);
            groups.Sort();
            return groups;
        }


        public Dictionary<string, int> GetGroupMembers(string groupname, int threshold)
        {

            // Enumerate group.
            string queryExpression;

            if (groupname == _allKeysGroupName)
                queryExpression = @"/KeycodeData/keycodes[group!='Unmappable Keys']";
            else if (groupname == _commonlyUsedKeysGroupName)
                queryExpression = @"/KeycodeData/keycodes[useful>='2'" + "]";
            else
                queryExpression = @"/KeycodeData/keycodes[group='" + groupname + "' and useful>='" + threshold.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "']";

            XPathNodeIterator iterator;

            iterator = (XPathNodeIterator)_navigator.Select(queryExpression);

            // Gives us a bunch of keycode nodes.
            // Given the scancode / extended from each node, ask for the name from the current layout.
            int scancode, extended;

            Dictionary<string, int> dir = new Dictionary<string, int>(iterator.Count);

            foreach (XPathNavigator node in iterator)
            {
                scancode = Int32.Parse(GetElementValue("sc", node), CultureInfo.InvariantCulture.NumberFormat);
                extended = Int32.Parse(GetElementValue("ex", node), CultureInfo.InvariantCulture.NumberFormat);
                string name = AppController.GetKeyName(scancode, extended);
                if (dir.ContainsKey(name)) // ArgumentException results when trying to add duplicate key..
                    Console.WriteLine("Duplicate name error: Name {0} Existing Scancode : {1} Scancode: {2}", name, dir[name], scancode);
                else
                    dir.Add(name, AppController.GetHashFromKeyData(scancode, extended));
            }

            return dir;
        }

        public List<int> LocalizableKeys
        {
            get
            {
                return GetKeys(true);
            }
        }

        public List<int> NonLocalizableKeys
        {
            get
            {
                return GetKeys(false);
            }
        }

        #endregion

        private List<int> GetKeys(bool localizable)
        {
            string expression = @"/KeycodeData/keycodes[localize = '" + (localizable ? "true" : "false") + "']";

            XPathNodeIterator iterator = (XPathNodeIterator)_navigator.Select(expression);

            List<int> keys = new List<int>(iterator.Count);

            for (int i = 0; i < iterator.Count; i++)
            {
                iterator.MoveNext();
                int scancode = Int32.Parse(GetElementValue("sc", iterator.Current), CultureInfo.InvariantCulture.NumberFormat);
                int extended = Int32.Parse(GetElementValue("ex", iterator.Current), CultureInfo.InvariantCulture.NumberFormat);
                keys.Add(AppController.GetHashFromKeyData(scancode, extended));
            }

            return keys;

        }

        public string GetKeyNameFromCode(int code)
        {
            int scancode = AppController.GetScancodeFromHash(code);
            int extended = AppController.GetExtendedFromHash(code);

            string expression = @"/KeycodeData/keycodes[sc = '" + scancode.ToString(CultureInfo.InvariantCulture.NumberFormat) + "' and ex = '" + extended.ToString(CultureInfo.InvariantCulture.NumberFormat) + "'] ";

            XPathNodeIterator iterator = (XPathNodeIterator)_navigator.Select(expression);

            string name = String.Empty;

            if (iterator.Count == 1)
            {
                iterator.MoveNext();
                name = GetElementValue("name", iterator.Current);
            }

            return name;

        }
    }
}
