using Crosstalk.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Convertors
{
    public class PartialConvertorProxy : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var partial = (Partial)value;
            partial.Convertor.WriteJson(writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var proxied = (Partial) Activator.CreateInstance(objectType);
            return proxied.Convertor.ReadJson(reader, objectType, existingValue, serializer); 
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Partial).IsAssignableFrom(objectType);
        }
    }
}
