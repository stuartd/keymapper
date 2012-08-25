using System.Diagnostics.CodeAnalysis;

namespace KeyMapper.Classes
{
    public class KeyHasher
    {
        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", 
            MessageId = "Overflow doesn't matter for this method - http://stackoverflow.com/a/892640")]
        public static int GetHashFromKeyData(int scancode, int extended)
        {
            // Need to preserve the actual extended value as they are all 224 except Pause
            // which is 225.
            return (scancode * 1000) + extended;
        }

        public static int GetScancodeFromHash(int hash)
        {
            return (hash / 1000);
        }

        public static int GetExtendedFromHash(int hash)
        {
            // Extended value is 224 when set.
            return (hash % 1000);
        }
    }
}
