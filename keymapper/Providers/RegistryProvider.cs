using System;
using KeyMapper.Classes;
using Microsoft.Win32;

namespace KeyMapper.Providers
{
    public static class RegistryProvider
    {
        public static bool GetRegistryLocation(MapLocation which, ref RegistryHive hive, ref string keyname, ref string valuename)
        {
            hive = RegistryHive.CurrentUser;

            switch (which)
            {
                case MapLocation.LocalMachineKeyboardLayout:
                    hive = RegistryHive.LocalMachine;
                    keyname = @"SYSTEM\CurrentControlSet\Control\Keyboard Layout";
                    valuename = "Scancode Map";
                    break;
                case MapLocation.CurrentUserKeyboardLayout:
                    keyname = @"Keyboard Layout";
                    valuename = "Scancode Map";
                    break;
                case MapLocation.KeyMapperLocalMachineKeyboardLayout:
                    keyname = AppController.ApplicationRegistryKeyName;
                    valuename = "BootMaps";
                    break;
                case MapLocation.KeyMapperCurrentUserKeyboardLayout:
                    keyname = AppController.ApplicationRegistryKeyName;
                    valuename = "UserMaps";
                    break;
                case MapLocation.KeyMapperVistaMappingsCache:
                    keyname = AppController.ApplicationRegistryKeyName;
                    valuename = "VistaBootCache";
                    break;
                default:
                    return false;
            }

            return true;
        }

        public static byte[] GetScancodeMapFromRegistry(MapLocation which)
        {
            RegistryKey registry = null;
            RegistryHive hive = RegistryHive.CurrentUser;
            string keyname = string.Empty;
            string valuename = string.Empty;

            if (GetRegistryLocation(which, ref hive, ref keyname, ref valuename))
            {
                switch (hive)
                {
                    case RegistryHive.LocalMachine:
                        registry = Registry.LocalMachine.OpenSubKey(keyname);
                        break;
                    case RegistryHive.CurrentUser:
                        registry = Registry.CurrentUser.OpenSubKey(keyname);
                        break;
                }
            }

            if (registry == null)
                return null;

            object keyvalue = registry.GetValue(valuename, null);

            if (keyvalue == null ||
                registry.GetValueKind(valuename) != RegistryValueKind.Binary ||
                keyvalue.GetType() != Type.GetType("System.Byte[]"))
            {
                // Not there, or not the right type.
                return null;
            }

            // Can't see how this cast can fail, shrug, will return null anyway.
            byte[] bytecodes = keyvalue as byte[];

            return bytecodes;
        }
    }
}
