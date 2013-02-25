using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Crosstalk.Common.Models;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Common.Convertors
{
    class PartialConvertor<T> : JsonConverter where T : class
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Partial<T>) value;
            serializer.Serialize(writer, obj.Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var res = new Partial<T>
            {
                Id = obj.GetValue("Id").Value<string>()
            };
            return res;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Partial<T>).IsAssignableFrom(objectType);
        }
    }
}
