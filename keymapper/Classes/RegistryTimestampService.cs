using System;
using Microsoft.Win32;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Classes
{
    public class RegistryTimestampService : IRegistryTimestampService
    {
        private const int KEY_QUERY_VALUE = 0x0001;
        private const int KEY_SET_VALUE = 0x0002;

        public DateTime GetRegistryKeyTimestamp(RegistryHive hive, string keyName)
        {
            long ts = GetRawRegistryKeyTimestamp(hive, keyName);

            var dt = (ts != 0 ? DateTime.FromFileTimeUtc(ts) : DateTime.MinValue);

            return dt.ToLocalTime();
        }

        private long GetRawRegistryKeyTimestamp(RegistryHive hive, string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return 0; // Otherwise the function opens HKLM (or HKCU) again.
            }

            var hKey = OpenKey(hive, keyName, KEY_QUERY_VALUE);

            if (hKey == UIntPtr.Zero)
            {
                return 0; // Didn't open key
            }

            uint result2 = NativeMethods.RegQueryInfoKey(
                hKey, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, out long keyTimestamp);

            if (result2 != 0)
            {
                keyTimestamp = 0; // Failed, don't return whatever value was supplied.
            }

            NativeMethods.RegCloseKey(hKey);

            return keyTimestamp;
        }

        public bool CanUserWriteToKey(RegistryHive hive, string keyName)
        {
            var hKey = OpenKey(hive, keyName, KEY_SET_VALUE);
            if (hKey == UIntPtr.Zero)
            {
                return false;
            }

            NativeMethods.RegCloseKey(hKey);
            return true;
        }

        private UIntPtr OpenKey(RegistryHive hive, string keyName, int requiredAccess)
        {
            UIntPtr hivePointer;

            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    hivePointer = (UIntPtr)0x80000000;
                    break;
                case RegistryHive.CurrentUser:
                    hivePointer = (UIntPtr)0x80000001;
                    break;
                case RegistryHive.LocalMachine:
                    hivePointer = (UIntPtr)0x80000002;
                    break;
                case RegistryHive.Users:
                    hivePointer = (UIntPtr)0x80000003;
                    break;
                case RegistryHive.CurrentConfig:
                    hivePointer = (UIntPtr)0x80000005;
                    break;
                default:
                    return UIntPtr.Zero;
            }

            int result = NativeMethods.RegOpenKeyEx(hivePointer, keyName, 0, requiredAccess, out var hKey);

            if (result == 0)
            {
                return hKey;
            }

            return UIntPtr.Zero;
        }
    }

}
