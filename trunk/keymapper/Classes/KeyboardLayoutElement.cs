namespace KeyMapper.Classes
{
    public class KeyboardLayoutElement
    {
        int _scancode;
        int _extended;
        BlankButton _button;
        int _horizontalstretch;
        int _verticalstretch;
        int _rightpadding;

        public int Scancode
        {
            get { return this._scancode; }
        }

        public int Extended
        {
            get { return this._extended; }
        }

        public BlankButton Button
        {
            get { return this._button; }
        }

        public int HorizontalStretch
        {
            get { return this._horizontalstretch; }
        }

        public int VerticalStretch
        {
            get { return this._verticalstretch; }
        }

        public int RightPadding
        {
            get { return this._rightpadding; }
        }


        public KeyboardLayoutElement(int scancode, int extended, BlankButton button,
                                     int horizontalStretch, int verticalStretch, int rightPadding)
        {
            this._scancode = scancode;
            this._extended = extended;
            this._button = button;
            this._horizontalstretch = horizontalStretch;
            this._verticalstretch = verticalStretch;
            this._rightpadding = rightPadding;
        }

    }
}