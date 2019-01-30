namespace KeyMapper.Classes
{
    public class KeyboardLayoutElement
    {
        private int _scanCode;
        private int _extended;
        private BlankButton _button;
        private int _horizontalstretch;
        private int _verticalstretch;
        private int _rightpadding;

        public int ScanCode
        {
            get { return _scanCode; }
        }

        public int Extended
        {
            get { return _extended; }
        }

        public BlankButton Button
        {
            get { return _button; }
        }

        public int HorizontalStretch
        {
            get { return _horizontalstretch; }
        }

        public int VerticalStretch
        {
            get { return _verticalstretch; }
        }

        public int RightPadding
        {
            get { return _rightpadding; }
        }


        public KeyboardLayoutElement(int scanCode, int extended, BlankButton button,
                                     int horizontalStretch, int verticalStretch, int rightPadding)
        {
            _scanCode = scanCode;
            _extended = extended;
            _button = button;
            _horizontalstretch = horizontalStretch;
            _verticalstretch = verticalStretch;
            _rightpadding = rightPadding;
        }

    }
}