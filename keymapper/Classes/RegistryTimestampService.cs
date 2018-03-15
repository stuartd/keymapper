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

            var dt = ts != 0 ? DateTime.FromFileTimeUtc(ts) : DateTime.MinValue;

            return dt.ToLocalTime();
        }

        private long GetRawRegistryKeyTimestamp(RegistryHive hive, string keyname)
        {
            if (string.IsNullOrEmpty(keyname))
            {
                return 0; // Otherwise the function opens HKLM (or HKCU) again.
            }

            var hkey = OpenKey(hive, keyname, KEY_QUERY_VALUE);

            if (hkey == UIntPtr.Zero)
            {
                return 0; // Didn't open key
            }

			uint result2 = NativeMethods.RegQueryInfoKey(
                hkey, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, out long timestamp);

            if (result2 != 0)
            {
                timestamp = 0; // Failed, don't return whatever value was supplied.
            }

            NativeMethods.RegCloseKey(hkey);

            return timestamp;
        }

        public bool CanUserWriteToKey(RegistryHive hive, string keyName)
        {
            var hkey = OpenKey(hive, keyName, KEY_SET_VALUE);
            if (hkey == UIntPtr.Zero)
            {
                return false;
            }

            NativeMethods.RegCloseKey(hkey);
            return true;
        }

        private UIntPtr OpenKey(RegistryHive hive, string keyname, int requiredAccess)
        {
            UIntPtr hiveptr;

			switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    hiveptr = (UIntPtr)0x80000000;
                    break;
                case RegistryHive.CurrentUser:
                    hiveptr = (UIntPtr)0x80000001;
                    break;
                case RegistryHive.LocalMachine:
                    hiveptr = (UIntPtr)0x80000002;
                    break;
                case RegistryHive.Users:
                    hiveptr = (UIntPtr)0x80000003;
                    break;
                case RegistryHive.CurrentConfig:
                    hiveptr = (UIntPtr)0x80000005;
                    break;
                default:
                    return UIntPtr.Zero;
            }

            int result = NativeMethods.RegOpenKeyEx(hiveptr, keyname, 0, requiredAccess, out var hkey);

            if (result == 0) {
				return hkey;
			}

			return UIntPtr.Zero;
        }
    }

}

