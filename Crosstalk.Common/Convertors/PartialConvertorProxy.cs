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
        private JsonConverter GetConvertor(Type type)
        {
            if (!type.IsGenericType) {
                throw new ArgumentException("Type is not generic", "type");
            }
            return (JsonConverter) Activator.CreateInstance(typeof(PartialConvertor<>).MakeGenericType(type.GetGenericArguments()));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            this.GetConvertor(value.GetType()).WriteJson(writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return this.GetConvertor(objectType).ReadJson(reader, objectType, existingValue, serializer); 
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Partial<>).IsAssignableFrom(objectType);
        }
    }
}
