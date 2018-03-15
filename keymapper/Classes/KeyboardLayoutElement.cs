namespace KeyMapper.Classes
{
    public class KeyboardLayoutElement
    {
		public int Scancode { get; }

		public int Extended { get; }

		public BlankButton Button { get; }

		public int HorizontalStretch { get; }

		public int VerticalStretch { get; }

		public int RightPadding { get; }


		public KeyboardLayoutElement(int scancode, int extended, BlankButton button,
                                     int horizontalStretch, int verticalStretch, int rightPadding)
        {
            Scancode = scancode;
            Extended = extended;
            Button = button;
            HorizontalStretch = horizontalStretch;
            VerticalStretch = verticalStretch;
            RightPadding = rightPadding;
        }

    }
}