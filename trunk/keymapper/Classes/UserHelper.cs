//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Runtime.InteropServices;

//namespace KeyMapper
//{
//    class UserHelper
//    {

//        // Returns True if connected to a Domain, and False if connected to a Workgroup or anything else.

//        public static bool IsConnectedToDomain()
//        {
//            int result = 0;
//            IntPtr pDomain = IntPtr.Zero;
//            NetJoinStatus status = NetJoinStatus.NetSetupUnknownStatus;

//            bool connectedToDomain = false;
//            try
//            {
//                result = NativeMethods.NetGetJoinInformation(null, out pDomain, out status);
//                if (result == 0 && status == NetJoinStatus.NetSetupDomainName)
//                {
//                    connectedToDomain = true;
//                }
//            }
//            finally
//            {
//                if (pDomain != IntPtr.Zero)
//                    NativeMethods.NetApiBufferFree(pDomain);
//            }

//            return connectedToDomain;
//        }


//        // NetGetJoinInformation() Enumeration
//        public enum NetJoinStatus
//        {
//            NetSetupUnknownStatus = 0,
//            NetSetupUnjoined,
//            NetSetupWorkgroupName,
//            NetSetupDomainName
//        } // NETSETUP_JOIN_STATUS



//        internal class NativeMethods
//        {
//            private NativeMethods() { }

//            [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
//            internal static extern int NetGetJoinInformation(
//              [In, MarshalAs(UnmanagedType.LPWStr)] string server,
//              out IntPtr domain,
//              out NetJoinStatus status);

//            [DllImport("Netapi32.dll")]
//            internal static extern int NetApiBufferFree(IntPtr Buffer);

//        }




//    }
//}
