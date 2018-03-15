using System;

namespace KeyMapper.Classes
{
    public class KeyMapping
	{
		public Key From { get; }

		public Key To { get; }

		public MappingType MapType { get; private set; }

		public KeyMapping()
        {
            From = new Key();
            To = new Key(); 
        }

		public KeyMapping(Key keyFrom, Key keyTo)
		{
            if (ReferenceEquals(keyFrom, null) || ReferenceEquals(keyTo, null))
            {
                throw new NullReferenceException("Key can't be null");
            }

		    From = keyFrom;
			To = keyTo;
			MapType = MappingType.Null;
		}

		public override string ToString()
		{
			return MappingDescription;
		}

		public void SetType(MappingType newType)
		{
            MapType = newType;
		}

        public string MappingDescription
        {
            get
            {
                // A mapping can be:
				bool disabled; // Is the mapping disabled or to a key?
                bool usermapping = MapType == MappingType.User; // User or Boot mapping? 
                string description = string.Empty;

                if (MappingsManager.IsMapped(this, MappingFilter.All) == false)
                {
                    // This 'mapping' is not currently mapped, so it must have been mapped previously and cleared.
                    var km = MappingsManager.GetClearedMapping(From.Scancode, From.Extended,
                                                                      MappingFilter.All);
                    if (MappingsManager.IsEmptyMapping(km) == false)
                    {
                        disabled = MappingsManager.IsDisabledMapping(km);

                        description = From.Name + (disabled ? " will be enabled" : " will be unmapped");
                        description += usermapping ? " when you next log on" : " after a restart";
                    }
                }
                else
                {
                    // So, mapped to something.
                    // Need to also know if it's Current or Pending

					bool pending = MappingsManager.IsMappingPending(this, usermapping ? MappingFilter.User : MappingFilter.Boot);

					disabled = MappingsManager.IsDisabledMapping(this);

                    description = From.Name + (pending ? " will be" : " is");
                    description += disabled ? " disabled" : " mapped to " + To.Name;

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
            return From.Scancode == 0 && To.Scancode == 0 && From.Extended == 0 && To.Extended == 0;
		}

		public bool IsValid()
		{
			// To be a valid mapping, From.Scancode must be greater than zero (to be a key)
			// and To.Scancode must be at least zero (either disabled or a key)

			// (Key has to able to be mapped to itself so user mappings can override boot mappings)

            return !IsEmpty()
				   && From.Scancode > 0
				   && To.Scancode > -1;

        }

		public static bool operator ==(KeyMapping map1, KeyMapping map2)
		{
			return map1.From == map2.From && map1.To == map2.To;
		}

		public override bool Equals(object obj)
		{
            if (obj.GetType() != GetType()) {
				return false;
			}

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



