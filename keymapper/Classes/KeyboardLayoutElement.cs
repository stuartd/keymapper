namespace KeyMapper.Classes
{
    public class KeyboardLayoutElement
    {
        public int ScanCode { get; }
        
        public int Extended { get; }

        public BlankButton Button { get; }

        public int HorizontalStretch { get; }

        public int VerticalStretch { get; }

        public int RightPadding { get; }

        public KeyboardLayoutElement(int scanCode, int extended, BlankButton button,
                                     int horizontalStretch, int verticalStretch, int rightPadding)
        {
            ScanCode = scanCode;
            Extended = extended;
            Button = button;
            HorizontalStretch = horizontalStretch;
            VerticalStretch = verticalStretch;
            RightPadding = rightPadding;
        }
    }
}