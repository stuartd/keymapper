namespace KeyMapper.Classes
{
    public class Key
    {
        public string Name { get; private set; }

        public int Scancode { get; }

        public int Extended { get; }

        public Key()
        {
            Name = string.Empty;
        }

        public Key(int scancode, int extended, string name)
        {
            Name = name;
            Scancode = scancode;
            Extended = extended;
        }

        public Key(int scancode, int extended)
            : this(scancode, extended, AppController.GetKeyName(scancode, extended))
        { }


        public override string ToString()
        {
            return AppController.GetKeyName(Scancode, Extended);
        }

        public static bool operator ==(Key key1, Key key2)
        {
            // If Scancode and Extended are the same, it's the same key.
            return (key1.Scancode == key2.Scancode && key1.Extended == key2.Extended);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;

            return this == (Key)obj;
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

