namespace KeyMapper.Classes
{
    public class Key
    {
        public string Name { get; }

        public int ScanCode { get; }

        public int Extended { get; }

        public Key()
        {
            Name = string.Empty;
        }

        public Key(int scanCode, int extended, string name)
        {
            Name = name;
            ScanCode = scanCode;
            Extended = extended;
        }

        public Key(int scanCode, int extended)
            : this(scanCode, extended, AppController.GetKeyName(scanCode, extended))
        {
        }


        public override string ToString()
        {
            return AppController.GetKeyName(ScanCode, Extended);
        }

        public static bool operator ==(Key key1, Key key2)
        {
            // If ScanCode and Extended are the same, it's the same key.
            return key1.ScanCode == key2.ScanCode && key1.Extended == key2.Extended;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return this == (Key)obj;
        }

        public override int GetHashCode()
        {
            return KeyHasher.GetHashFromKey(this);
        }

        public static bool operator !=(Key key1, Key key2)
        {
            return !(key1 == key2);
        }
    }
}
