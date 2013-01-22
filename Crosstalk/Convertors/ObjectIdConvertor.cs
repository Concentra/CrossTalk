using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Crosstalk.Convertors
{
    public class ObjectIdConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((MongoDB.Bson.ObjectId)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new MongoDB.Bson.ObjectId((string)existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (MongoDB.Bson.ObjectId) == objectType;
        }
    }
}