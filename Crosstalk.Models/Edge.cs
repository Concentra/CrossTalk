using System;
using Crosstalk.Core.Models.Messages.Channels;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Models.Messages
{
    public class Edge
    {
        public long Id { get; set; }
        
        [BsonIgnore]
        public Identity From;
        
        [BsonIgnore]
        public Identity To;
        
        [BsonIgnore]
        public bool IsPartial
        {
            get { return null == this.From && null == this.To; }
        }

        [BsonIgnore]
        [JsonConverter(typeof(ChannelTypeConvertor))]
        public ChannelType Type { get; set; }
        
    }

    class ChannelTypeConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value as ChannelType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (ChannelType) JToken.Load(reader).Value<String>();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (ChannelType).IsAssignableFrom(objectType);
        }
    }
}