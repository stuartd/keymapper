using System.Collections.Generic;
using KeyMapper.Classes;

namespace KeyMapper.Interfaces
{
    public interface IKeyData
    {
        // Keylists. Both of these added together make up the usual 104 keyboard.
        IList<int> LocalizableKeys { get; }

        IList<int> NonLocalizableKeys { get; }

        IEnumerable<string> GetGroupList(int threshold);

        Dictionary<string, int> GetGroupMembers(string group, int threshold);

        // Keyboard layout.
        KeyboardLayoutType GetKeyboardLayoutType(string locale);
    }
}
