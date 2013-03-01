using Crosstalk.Common.Convertors;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public class Report
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public Partial Parent { get; set; }
        public string Type { get; set; }
    }
}
