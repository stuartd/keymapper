using System;
using System.Collections.Generic;
using System.Text;

namespace KeyMapper
{
    // From http://bartdesmet.net/blogs/bart/archive/2006/09/26/4470.aspx

    [Flags]
    public enum TaskDialogButtons
    {
        OK = 0x0001,
        Cancel = 0x0008,
        Yes = 0x0002,
        No = 0x0004,
        Retry = 0x0010,
        Close = 0x0020
    }

    public enum TaskDialogIcon
    {
        Information = UInt16.MaxValue - 2,
        Warning = UInt16.MaxValue,
        Stop = UInt16.MaxValue - 1,
        Question = 0,
        SecurityWarning = UInt16.MaxValue - 5,
        SecurityError = UInt16.MaxValue - 6,
        SecuritySuccess = UInt16.MaxValue - 7,
        SecurityShield = UInt16.MaxValue - 3,
        SecurityShieldBlue = UInt16.MaxValue - 4,
        SecurityShieldGray = UInt16.MaxValue - 8
    }

    public enum TaskDialogResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No,
        Retry,
        Close
    }
}

