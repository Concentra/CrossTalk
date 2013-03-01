using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crosstalk.Common.Models
{
    public sealed class ReportType
    {
        private readonly string _name;
        private static readonly Dictionary<string, ReportType> instances = new Dictionary<string, ReportType>();  

        public static readonly ReportType Message = new ReportType("message");
        public static readonly ReportType Comment = new ReportType("comment");

        private ReportType(string name)
        {
            this._name = name;
            instances[name] = this;
        }

        public override string ToString()
        {
            return this._name;
        }

        public static implicit operator ReportType(string str)
        {
            ReportType type;
            if (instances.TryGetValue(str, out type))
            {
                return type;
            }
            throw new InvalidCastException("\"" + str + "\" could not be cast to a ReportType");
        }

        public static implicit operator string(ReportType type)
        {
            return type.ToString();
        }
    }
}
