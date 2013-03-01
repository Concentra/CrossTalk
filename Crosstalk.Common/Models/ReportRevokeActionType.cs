using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public sealed class ReportRevokeActionType
    {
        private readonly string _name;
        private static readonly Dictionary<string, ReportRevokeActionType> instances = new Dictionary<string, ReportRevokeActionType>();  

        public static readonly ReportRevokeActionType Revoke = new ReportRevokeActionType("revoke");
        public static readonly ReportRevokeActionType Moderate = new ReportRevokeActionType("moderate");

        private ReportRevokeActionType(string name)
        {
            this._name = name;
            instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator ReportRevokeActionType(string str)
        {
            ReportRevokeActionType type;
            if (instances.TryGetValue(str, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + str + "\" could not be cast to a ReportType");
        }

        public static implicit operator string(ReportRevokeActionType type)
        {
            return type.ToString();
        }
    }
}
