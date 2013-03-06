using Crosstalk.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Convertors
{
    public class ExpandingPartialConvertor : PartialConvertorProxy
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dynamic obj = value;
            serializer.Serialize(writer, obj.Value);
        }
    }
}
