using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public sealed class IdentityType
    {
        private readonly string _name;
        private readonly int _value;
        private static readonly Dictionary<string, IdentityType> Instances = new Dictionary<string, IdentityType>();  

        public static readonly IdentityType Person = new IdentityType(1, "person");
        public static readonly IdentityType Group = new IdentityType(2, "group");
        public static readonly IdentityType Public = new IdentityType(3, "public");

        private IdentityType(int value, string name)
        {
            this._name = name;
            this._value = value;
            Instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator IdentityType(string str)
        {
            IdentityType type;
            if (Instances.TryGetValue(str, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + str + "\" could not be cast to an IdentityType");
        }

        public static implicit operator string(IdentityType type)
        {
            return type.ToString();
        }

    }
}
