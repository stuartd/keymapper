using System;

namespace KeyMapper.Classes
{
    public class KeyMapperKeyPressedEventArgs : EventArgs
    {
        public KBHookStruct Key { get;}

        public KeyMapperKeyPressedEventArgs(KBHookStruct key)
        {
            Key = key;
        }
    }
}