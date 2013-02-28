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
        private static readonly Dictionary<string, ReportableStatus> instances = new Dictionary<string, ReportableStatus>();  

        public static readonly ReportableStatus None = new ReportableStatus("none");
        public static readonly ReportableStatus Reported = new ReportableStatus("reported");
        public static readonly ReportableStatus Removed = new ReportableStatus("removed");
        public static readonly ReportableStatus Revoked = new ReportableStatus("revoked");
        public static readonly ReportableStatus Missing = new ReportableStatus("missing");

        private ReportableStatus(string name)
        {
            this._name = name;
            instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator ReportableStatus(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return ReportableStatus.None;
            }
            ReportableStatus type;
            if (instances.TryGetValue(str, out type))
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
