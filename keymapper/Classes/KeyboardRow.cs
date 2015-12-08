using System.Collections.Generic;

namespace KeyMapper.Classes
{
    public class KeyboardRow
    {
        public List<KeyboardLayoutElement> Keys { get; private set; }

        public KeyboardRow(List<KeyboardLayoutElement> keys)
        {
            Keys = keys;
        }

    }

}
