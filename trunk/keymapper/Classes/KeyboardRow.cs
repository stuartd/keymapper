using System.Collections.Generic;

namespace KeyMapper.Classes
{
    public class KeyboardRow
    {
        readonly List<KeyboardLayoutElement> _keys;

        public IEnumerable<KeyboardLayoutElement> Keys
        {
            get { return _keys; }
        }

        public KeyboardRow(List<KeyboardLayoutElement> keys)
        {
            _keys = keys;
        }

    }

}
