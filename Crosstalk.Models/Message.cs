using Crosstalk.Core.Models.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Crosstalk.Core.Models
{
    public class Message
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConvertor))]
        public ObjectId Id { get; set; }
        public Edge Edge { get; set; }
        public string Body { get; set; }
    }
}