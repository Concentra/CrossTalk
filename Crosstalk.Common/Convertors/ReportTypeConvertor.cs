using Crosstalk.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Convertors
{
    public class ReportTypeConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((ReportType)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ((ReportType)serializer.Deserialize<string>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ReportType).IsAssignableFrom(objectType);
        }
    }
}
