using System;
using KeyMapper.Classes;
using Microsoft.Win32;

namespace KeyMapper.Providers
{
    public static class RegistryProvider
    {
        public static bool GetRegistryLocation(MapLocation which, ref RegistryHive hive, ref string keyName, ref string valueName)
        {
            hive = RegistryHive.CurrentUser;

            switch (which)
            {
                case MapLocation.LocalMachineKeyboardLayout:
                    hive = RegistryHive.LocalMachine;
                    keyName = @"SYSTEM\CurrentControlSet\Control\Keyboard Layout";
                    valueName = "ScanCode Map";
                    break;

                case MapLocation.KeyMapperLocalMachineKeyboardLayout:
                    keyName = AppController.ApplicationRegistryKeyName;
                    valueName = "Mappings";
                    break;

                case MapLocation.KeyMapperMappingsCache:
                    keyName = AppController.ApplicationRegistryKeyName;
                    valueName = "MappingCache";
                    break;

                default:
                    return false;
            }

            return true;
        }

        public static byte[] GetScanCodeMapFromRegistry(MapLocation which)
        {
            RegistryKey registry = null;
            var hive = RegistryHive.CurrentUser;
            string keyName = string.Empty;
            string valueName = string.Empty;

            if (GetRegistryLocation(which, ref hive, ref keyName, ref valueName))
            {
                switch (hive)
                {
                    case RegistryHive.LocalMachine:
                        registry = Registry.LocalMachine.OpenSubKey(keyName);
                        break;
                    case RegistryHive.CurrentUser:
                        registry = Registry.CurrentUser.OpenSubKey(keyName);
                        break;
                }
            }

            var keyValue = registry?.GetValue(valueName, null);

            if (keyValue == null ||
                registry.GetValueKind(valueName) != RegistryValueKind.Binary ||
                keyValue.GetType() != Type.GetType("System.Byte[]"))
            {
                // Not there, or not the right type.
                return null;
            }

            // Can't see how this cast can fail, shrug, will return null anyway.
            var bytecodes = keyValue as byte[];

            return bytecodes;
        }
    }
}
