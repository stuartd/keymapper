namespace KeyMapper.Classes
{
    public class KeyHasher
    {
        public static int GetHashFromKey(Key key)
        {
            return GetHashFromKeyData(key.ScanCode, key.Extended);
        }

        public static int GetHashFromKeyData(int scanCode, int extended)
        {
            // Need to preserve the actual extended value as they are all 224 except Pause
            // which is 225.
            return scanCode * 1000 + extended;
        }

        public static int GetScanCodeFromHash(int hash)
        {
            return hash / 1000;
        }

        public static int GetExtendedFromHash(int hash)
        {
            // Extended value is 224 when set.
            return hash % 1000;
        }
    }
}
