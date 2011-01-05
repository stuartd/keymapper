using System;
using System.Collections.Generic;
using System.Text;

namespace KeyMapper.Providers
{
    public static class OperatingSystemVersionProvider
    {
        // Need to be able to mock the operating system version.

        // Four our purposes, there are only four groups as scancode mapping was introduced in Windows 2000
        // 1) Windows 2000 (5.0)
        // 2) XP, Server 2003 (5.1, 5.2)
        // 3) Vista, Server 2008 (6.0, 6.1)
        // 4) Windows 7 and later (Server 2008 R2) (6.2 ??? TODO)

        // In reality, we only care if the operating system supports user mappings
        // i.e. XP and Vista

        private static int _majorVersion;
        private static int _minorVersion;

        static OperatingSystemVersionProvider()
        {
            _majorVersion = Environment.OSVersion.Version.Major;
            _minorVersion = Environment.OSVersion.Version.Minor;
        }

        public static void Mock(int majorVersion, int minorVersion)
        {
            _majorVersion = majorVersion;
            _minorVersion = minorVersion;
        }

        private static bool IsWindows2000
        {
            get
            {
                return (_majorVersion < 5
                        ||
                        (_majorVersion == 5 && _minorVersion == 0));
            }
        }

        private static bool IsXP
        {
            get
            {
                return _majorVersion == 5 && _minorVersion == 1;
            }
        }

        private static bool IsVista
        {
            get
            {
                return _majorVersion == 6 && _minorVersion == 0;
            }
        }

        private static bool IsWindows7OrLater
        {
            get
            {
                return _majorVersion > 6
                       || (_majorVersion == 6 && _minorVersion > 0);

            }
        }

        public static string OperatingSystem
        {
            get
            {
                return "Windows " + (IsVista ? "Vista" : IsWindows2000 ? "2000" : IsXP ? "XP" : "7");
            }
        }

        public static bool OperatingSystemSupportsUserMappings
        {
            get { return IsXP || IsVista; }
        }

        public static bool OperatingSystemImplementsUAC
        {
            get { return IsVista || IsWindows7OrLater; }
        }

        public static bool OperatingSystemImplementsTaskDialog
        {
            get { return (IsVista || IsWindows7OrLater); }
        }
    }
}
