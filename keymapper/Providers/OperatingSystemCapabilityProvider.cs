using System;

namespace KeyMapper.Providers
{
    public class OperatingSystemCapabilityProvider : IOperatingSystemCapability 
    {
        // 0) < 5 - Pre Win2k - program will not work. Scancode mapping was introduced in Win2k

        // Windows 2000 (5.0)
        // XP, Server 2003 (5.1)
        // Vista, Server 2008 (6.0)
        // Windows 7 and Server 2008 R2 (6.1)
        // Windows 8 and Server 2012 (6.2)

        // Anyway, interested in capabilities, not versions.

        private readonly int majorVersion;
        private readonly int minorVersion;

        private OperatingSystemCapabilityProvider(int mockMajorVersion, int mockMinorVersion)
        {
            this.majorVersion = mockMajorVersion;
            this.minorVersion = mockMinorVersion;
        }

        public IOperatingSystemCapability CreateMockInstance(int mockMajorVersion, int mockMinorVersion)
        {
            return new OperatingSystemCapabilityProvider(mockMajorVersion, mockMinorVersion);
        }

        public OperatingSystemCapabilityProvider()
        {
            majorVersion = Environment.OSVersion.Version.Major;
            minorVersion = Environment.OSVersion.Version.Minor;
        }

        public string OperatingSystem
        {
            get
            {
                return "Windows " + (isVista ? "Vista" : isWindows2000 ? "2000" : this.isXp ? "XP" : isWindows7 ? "7" : isWindows8 ? "8" : "Mystery Edition");
            }
        }

        /// <remarks>Only supported by XP and Vista</remarks>
        public bool SupportsUserMappings
        {
            get { return this.isXp || isVista; }
        }

        /// <remarks>XP and later</remarks>
        public bool SupportsLocalizedKeyboardNames
        {
            get { return isXpOrLater; }
        }

        /// <remarks>Vista and later</remarks>
        public bool ImplementsUAC
        {
            get { return isVistaOrLater ; }
        }

        /// <remarks>Vista and later</remarks>
        public bool ImplementsTaskDialog
        {
            get { return isVistaOrLater; }
        }

        private bool isWindows2000
        {
            get
            {
                return (majorVersion == 5 && minorVersion == 0);
            }
        }

        private bool isXp
        {
            get
            {
                return majorVersion == 5 && minorVersion == 1;
            }
        }

        private bool isVista
        {
            get
            {
                return majorVersion == 6 && minorVersion == 0;
            }
        }

        private bool isWindows7
        {
            get
            {
                return majorVersion == 6 && minorVersion == 1;
            }
        }

        private bool isWindows8
        {
            get
            {
                return majorVersion == 6 && minorVersion == 2;
            }
        }

        private bool isXpOrLater
        {
            get
            {
                return majorVersion > 5 || isXp;
            }
        }

        private bool isVistaOrLater
        {
            get { return majorVersion > 5; }
        }
    }
}
