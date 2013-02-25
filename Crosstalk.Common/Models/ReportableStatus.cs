using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public sealed class ReportableStatus
    {
        private readonly string _name;
        private readonly int _value;
        private static readonly Dictionary<string, ReportableStatus> Instances = new Dictionary<string, ReportableStatus>();  

        public static readonly ReportableStatus None = new ReportableStatus(1, "none");
        public static readonly ReportableStatus Reported = new ReportableStatus(2, "reported");
        public static readonly ReportableStatus Removed = new ReportableStatus(3, "removed");
        public static readonly ReportableStatus Revoked = new ReportableStatus(4, "revoked");

        private ReportableStatus(int value, string name)
        {
            this._name = name;
            this._value = value;
            Instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator ReportableStatus(string str)
        {
            ReportableStatus type;
            if (Instances.TryGetValue(str, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + str + "\" could not be cast to a ReportableStatus");
        }

        public static implicit operator string(ReportableStatus type)
        {
            return type.ToString();
        }
    }
}
