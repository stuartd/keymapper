using System;

namespace KeyMapper.Classes
{
    public class KeyMapping
    {
        public Key From { get; }

        public Key To { get; }

        public KeyMapping() : this(new Key(), new Key())
        {
        }

        public KeyMapping(Key keyFrom, Key keyTo)
        {
            if (ReferenceEquals(keyFrom, null) || ReferenceEquals(keyTo, null))
            {
                throw new NullReferenceException("Key can't be null");
            }

            From = keyFrom;
            To = keyTo;
        }

        public override string ToString()
        {
            return MappingDescription;
        }

        public string MappingDescription
        {
            get
            {
                bool disabled; // Is the mapping disabled or to a key?

                string description = string.Empty;

                if (MappingsManager.IsMapped(this) == false)
                {
                    // This 'mapping' is not currently mapped, so it must have been mapped previously and cleared.
                    var km = MappingsManager.GetClearedMapping(From.ScanCode, From.Extended);
                    if (MappingsManager.IsEmptyMapping(km) == false)
                    {
                        disabled = MappingsManager.IsDisabledMapping(km);

                        description = From.Name + (disabled ? " will be enabled" : " will be unmapped") + " after a restart";
                    }
                }
                else
                {
                    // So, mapped to something.
                    // Need to also know if it's Current or Pending

                    bool pending = MappingsManager.IsMappingPending(this);

                    disabled = MappingsManager.IsDisabledMapping(this);

                    description = From.Name + (pending ? " will be" : " is");
                    description += disabled ? " disabled" : " mapped to " + To.Name;

                    if (pending)
                    {
                        description += " after a restart";
                    }
                }

                return description;
            }
        }

        // This will match anything created by New KeyMapping() with no parameters
        public bool IsEmpty()
        {
            return From.ScanCode == 0 && To.ScanCode == 0 && From.Extended == 0 && To.Extended == 0;
        }

        public bool IsValid()
        {
            // To be a valid mapping, From.ScanCode must be greater than zero (to be a key)
            // and To.ScanCode must be at least zero (either disabled or a key)

            return !IsEmpty()
                && From.ScanCode > 0
                && To.ScanCode > -1;

        }

        public static bool operator ==(KeyMapping map1, KeyMapping map2)
        {
            return map1.From == map2.From && map1.To == map2.To;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return this == (KeyMapping)obj;
        }

        public static bool operator !=(KeyMapping map1, KeyMapping map2)
        {
            return !(map1 == map2);
        }

        public override int GetHashCode()
        {
            return KeyHasher.GetHashFromKey(From) * 31 + KeyHasher.GetHashFromKey(To);
        }
    }
}
