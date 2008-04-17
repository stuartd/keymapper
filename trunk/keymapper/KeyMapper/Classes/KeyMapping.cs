using System;

namespace KeyMapper
{

	public struct KeyMapping
	{

		#region fields, properties

		private Key _from;
		private Key _to;

		internal Key From
		{
			get { return _from; }
		}

		internal Key To
		{
			get { return _to; }
		}

		#endregion

		#region methods

		public KeyMapping(Key keyFrom, Key keyTo)
		{
			this._from = keyFrom;
			this._to = keyTo;
		}

		public override string ToString()
		{
			return MappingDescription();
		}

		public string MappingDescription()
		{
			// A mapping can be:
			bool _pending ; // Current or pending
			bool _usermapping ; // User or boot mapping
			bool _disabled ; // Is the mapping disabled or to a key?

			string description = String.Empty;

			if (MappingsManager.IsMapped(this, MappingFilter.All) == false)
			{
				// Not currently mapped. Was it perchance mapped previously and cleared?
				KeyMapping km = MappingsManager.GetClearedMapping(_from.Scancode, _from.Extended, MappingFilter.All);
				if (MappingsManager.IsEmptyMapping(km) == false)
				{
					_disabled = MappingsManager.IsDisabledMapping(km);
					_usermapping = MappingsManager.WasClearedMappingUserMapping(km);

					description = _from.Name + (_disabled ? " will be enabled" : " will be unmapped");
					description += _usermapping ? " when you next log on" : " after a restart";
				}
			}
			else
			{

				// So, mapped to something.
				// Need to also know if it's Current or Pending

				_pending = MappingsManager.IsMappingPending(this, MappingFilter.All);
				_usermapping = MappingsManager.IsMapped(this, MappingFilter.User);
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
				&& _from != null 
				&& _to != null 
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
			return (obj is KeyMapping && this == (KeyMapping)obj);
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

}



