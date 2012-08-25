using KeyMapper.Classes.Interop;
using System;

namespace KeyMapper.Classes
{
    public static class DpiInfo
    {
        static DpiInfo()
        {
            // X is - DpiX = NativeMethods.GetDeviceCaps(NativeMethods.GetDC(IntPtr.Zero), 88);
            // But we only use Y in calculations.
            // DpiY = NativeMethods.GetDeviceCaps(NativeMethods.GetDC(IntPtr.Zero), 90);

            Dpi = NativeMethods.GetDeviceCaps(NativeMethods.GetDC(IntPtr.Zero), 90);
        }

        public static int Dpi { get; set; }
    }
}
