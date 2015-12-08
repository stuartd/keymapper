using System;
using System.Collections.Generic;
using System.Text;

namespace KeyMapper.Classes.Interop
{
    // From http://bartdesmet.net/blogs/bart/archive/2006/09/26/4470.aspx

    [Flags]
    public enum TaskDialogButtons
    {
        Ok = 0x0001,
        Cancel = 0x0008,
        Yes = 0x0002,
        No = 0x0004,
        Retry = 0x0010,
        Close = 0x0020
    }

    public enum TaskDialogIcon
    {
        Information = ushort.MaxValue - 2,
        Warning = ushort.MaxValue,
        Stop = ushort.MaxValue - 1,
        Question = 0,
        SecurityWarning = ushort.MaxValue - 5,
        SecurityError = ushort.MaxValue - 6,
        SecuritySuccess = ushort.MaxValue - 7,
        SecurityShield = ushort.MaxValue - 3,
        SecurityShieldBlue = ushort.MaxValue - 4,
        SecurityShieldGray = ushort.MaxValue - 8
    }

    public enum TaskDialogResult
    {
        None,
        Ok,
        Cancel,
        Yes,
        No,
        Retry,
        Close
    }
}

