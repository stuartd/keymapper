using System.Collections.Generic;

namespace KeyMapper.Classes
{
    public class KeyboardRow
    {
        public List<KeyboardLayoutElement> Keys { get; }

        public KeyboardRow(List<KeyboardLayoutElement> keys)
        {
            Keys = keys;
        }

    }

}
