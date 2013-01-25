using System;
using Newtonsoft.Json;

namespace Crosstalk.Models.Convertors
{
    public class ObjectIdConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((MongoDB.Bson.ObjectId)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (typeof (MongoDB.Bson.ObjectId) == objectType)
            {
                return existingValue;
            }
            return new MongoDB.Bson.ObjectId((string)existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (MongoDB.Bson.ObjectId) == objectType || typeof(string) == objectType;
        }
    }
}