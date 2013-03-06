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
    class PartialConvertor<T> : JsonConverter where T : class, ISupportsPartial
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Partial<T>) value;
            if (obj.IsPartial)
            {
                serializer.Serialize(writer, new
                {
                    Id = obj.Id,
                    _Partial = true
                });
            }
            else
            {
                serializer.Serialize(writer, obj.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Partial<T> res;

            var obj = serializer.Deserialize<dynamic>(reader);
            if (null == obj)
            {
                return null;
            }

            if (null != obj._Partial)
            {
                res = new Partial<T>
                {
                    Id = obj.Id.ToObject<string>()
                };
            }
            else
            {
                res = new Partial<T>(obj.ToObject<T>());
            }

            return res;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Partial<T>).IsAssignableFrom(objectType);
        }
    }
}
