namespace KeyMapper.Classes
{
    public class KeyboardLayoutElement
    {
        private int _scancode;
        private int _extended;
        private BlankButton _button;
        private int _horizontalstretch;
        private int _verticalstretch;
        private int _rightpadding;

        public int Scancode
        {
            get { return _scancode; }
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


        public KeyboardLayoutElement(int scancode, int extended, BlankButton button,
                                     int horizontalStretch, int verticalStretch, int rightPadding)
        {
            _scancode = scancode;
            _extended = extended;
            _button = button;
            _horizontalstretch = horizontalStretch;
            _verticalstretch = verticalStretch;
            _rightpadding = rightPadding;
        }

    }
}