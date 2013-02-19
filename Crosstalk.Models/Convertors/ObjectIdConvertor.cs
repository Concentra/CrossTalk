using System;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Models.Convertors
{
    public class ObjectIdConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((MongoDB.Bson.ObjectId)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string oid;
            if (typeof (MongoDB.Bson.ObjectId) == objectType)
            {
                var tmp = JToken.Load(reader);
                oid = tmp.Value<string>();
            }
            else
            {
                oid = (existingValue as string);
            }
            return ObjectId.Parse(oid);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (MongoDB.Bson.ObjectId) == objectType || typeof(string) == objectType;
        }
    }
}