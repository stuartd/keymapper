namespace KeyMapper
{
	public struct Key
	{

		private string _name;
		private int _scancode;
		private int _extended;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int Scancode
		{
			get { return _scancode; }
			set { _scancode = value; }
		}

		public int Extended
		{
			get { return _extended; }
			set { _extended = value; }
		}

		public Key(int scancode, int extended, string name)
		{
			_name = name;
			_scancode = scancode;
			_extended = extended;
		}

		public Key(int scancode, int extended)
			: this(scancode, extended, AppController.GetKeyName(scancode, extended))
		{ }

		public override string ToString()
		{
			return AppController.GetKeyName(_scancode, _extended);
		}

		public static bool operator ==(Key key1, Key key2)
		{
			// If Scancode and Extended are the same, it's the same key.
			return (key1.Scancode == key2.Scancode && key1.Extended == key2.Extended);
		}

		public override bool Equals(object obj)
		{
			return (obj is Key && this == (Key)obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// The C# compiler and rule OperatorsShouldHaveSymmetricalOverloads require this.
		public static bool operator !=(Key key1, Key key2)
		{
			return !(key1 == key2);
		}

	}
}

