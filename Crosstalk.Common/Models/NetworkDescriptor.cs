using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public sealed class NetworkDescriptor
    {
        private string _name;

        private NetworkDescriptor(string name)
        {
            this._name = name;
            instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator NetworkDescriptor(string str)
        {
            NetworkDescriptor type;
            if (instances.TryGetValue(str, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + str + "\" could not be cast to an IdentityType");
        }

        public static implicit operator string(NetworkDescriptor type)
        {
            return type.ToString();
        }


        private static Dictionary<string, NetworkDescriptor> instances = new Dictionary<string, NetworkDescriptor>();

        public static readonly NetworkDescriptor Public = new NetworkDescriptor("public");
        public static readonly NetworkDescriptor Network = new NetworkDescriptor("network");
        public static readonly NetworkDescriptor All = new NetworkDescriptor("all");
    }
}
