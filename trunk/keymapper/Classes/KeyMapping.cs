using System;

namespace KeyMapper.Classes
{

	public class KeyMapping
	{

		#region fields, properties

		private Key _from;
		private Key _to;
		private MappingType _type;

		public Key From
		{
			get { return _from; }
		}

		public Key To
		{
			get { return _to; }
		}

		public MappingType MapType
		{
			get { return _type; }
		}

		#endregion

		#region methods

        public KeyMapping()
        {
            _from = new Key();
            _to = new Key(); 
        }

		public KeyMapping(Key keyFrom, Key keyTo)
		{
            if (Object.ReferenceEquals(keyFrom, null) || Object.ReferenceEquals(keyTo, null))
                throw new NullReferenceException("Key can't be null");

			_from = keyFrom;
			_to = keyTo;
			_type = MappingType.Null;
		}

		public override string ToString()
		{
			return MappingDescription();
		}

		public void SetType(MappingType type)
		{
			_type = type;
		}

		public string MappingDescription()
		{
			// A mapping can be:
			bool _pending ; // Current or pending
			bool _disabled ; // Is the mapping disabled or to a key?
			bool _usermapping	= (_type == MappingType.User); // User or Boot mapping? 
			string description = String.Empty;

			if (MappingsManager.IsMapped(this, MappingFilter.All) == false)
			{
				// This 'mapping' is not currently mapped, so it must have been mapped previously and cleared.
				KeyMapping km = MappingsManager.GetClearedMapping(_from.Scancode, _from.Extended, MappingFilter.All);
				if (MappingsManager.IsEmptyMapping(km) == false)
				{
					_disabled = MappingsManager.IsDisabledMapping(km);
					
					description = _from.Name + (_disabled ? " will be enabled" : " will be unmapped");
					description += _usermapping ? " when you next log on" : " after a restart";
				}
			}
			else
			{

				// So, mapped to something.
				// Need to also know if it's Current or Pending

				if (_usermapping)
				_pending = MappingsManager.IsMappingPending(this, MappingFilter.User);
				else
					_pending = MappingsManager.IsMappingPending(this, MappingFilter.Boot);

				_disabled = MappingsManager.IsDisabledMapping(this);

				description = _from.Name + (_pending ? " will be" : " is");
				description += _disabled ? " disabled" : " mapped to " + _to.Name;
				if (_pending)
					description += _usermapping ? " when you next log on" : " after a restart";
			}

			return description;
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

            return (
            !IsEmpty()
            && _from.Scancode > 0
            && _to.Scancode > -1
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

		#endregion

	}

	public enum MappingType
	{
		Null, User, Boot
	}

}



