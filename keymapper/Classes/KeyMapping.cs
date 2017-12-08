using System;

namespace KeyMapper.Classes
{
    public class KeyMapping
	{
        private readonly Key @from;
		private readonly Key to;
		private MappingType type;

		public Key From
		{
			get { return this.@from; }
		}

		public Key To
		{
			get { return this.to; }
		}

		public MappingType MapType
		{
			get { return this.type; }
		}

        public KeyMapping()
        {
            this.@from = new Key();
            this.to = new Key(); 
        }

		public KeyMapping(Key keyFrom, Key keyTo)
		{
            if (ReferenceEquals(keyFrom, null) || ReferenceEquals(keyTo, null))
            {
                throw new NullReferenceException("Key can't be null");
            }

		    this.@from = keyFrom;
			this.to = keyTo;
			this.type = MappingType.Null;
		}

		public override string ToString()
		{
			return MappingDescription;
		}

		public void SetType(MappingType newType)
		{
            this.type = newType;
		}

        public string MappingDescription
        {
            get
            {
                // A mapping can be:
                bool pending; // Current or pending
                bool disabled; // Is the mapping disabled or to a key?
                bool usermapping = (this.type == MappingType.User); // User or Boot mapping? 
                string description = string.Empty;

                if (MappingsManager.IsMapped(this, MappingFilter.All) == false)
                {
                    // This 'mapping' is not currently mapped, so it must have been mapped previously and cleared.
                    KeyMapping km = MappingsManager.GetClearedMapping(this.@from.Scancode, this.@from.Extended,
                                                                      MappingFilter.All);
                    if (MappingsManager.IsEmptyMapping(km) == false)
                    {
                        disabled = MappingsManager.IsDisabledMapping(km);

                        description = this.@from.Name + (disabled ? " will be enabled" : " will be unmapped");
                        description += usermapping ? " when you next log on" : " after a restart";
                    }
                }
                else
                {
                    // So, mapped to something.
                    // Need to also know if it's Current or Pending

                    if (usermapping)
                    {
                        pending = MappingsManager.IsMappingPending(this, MappingFilter.User);
                    }
                    else
                    {
                        pending = MappingsManager.IsMappingPending(this, MappingFilter.Boot);
                    }

                    disabled = MappingsManager.IsDisabledMapping(this);

                    description = this.@from.Name + (pending ? " will be" : " is");
                    description += disabled ? " disabled" : " mapped to " + this.to.Name;

                    if (pending)
                    {
                        description += usermapping ? " when you next log on" : " after a restart";
                    }
                }

                return description;
            }
        }

        // This will match anything created by New KeyMapping() with no parameters
		public bool IsEmpty()
		{
            return (this.From.Scancode == 0 && this.To.Scancode == 0 && this.From.Extended == 0 && this.To.Extended == 0);
		}

		public bool IsValid()
		{
			// To be a valid mapping, From.Scancode must be greater than zero (to be a key)
			// and To.Scancode must be at least zero (either disabled or a key)

			// (Key has to able to be mapped to itself so user mappings can override boot mappings)

            return (!IsEmpty()
                  && this.@from.Scancode > 0
                  && this.to.Scancode > -1
            );

        }

		public static bool operator ==(KeyMapping map1, KeyMapping map2)
		{
			return (map1.From == map2.From && map1.To == map2.To);
		}

		public override bool Equals(object obj)
		{
            if (obj.GetType() != this.GetType())
                return false;

			return this == (KeyMapping)obj;
		}

        // The C# compiler and rule OperatorsShouldHaveSymmetricalOverloads require this.
		public static bool operator !=(KeyMapping map1, KeyMapping map2)
		{
			return !(map1 == map2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public enum MappingType
	{
		Null, User, Boot
	}

}



