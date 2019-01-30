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

        public int ScanCode => _scanCode;

        public int Extended => _extended;

        public BlankButton Button => _button;

        public int HorizontalStretch => _horizontalstretch;

        public int VerticalStretch => _verticalstretch;

        public int RightPadding => _rightpadding;


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