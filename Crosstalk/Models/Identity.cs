using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crosstalk.Models
{
    public class Identity
    {
        /// <summary>
        /// The data we actual care about
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConvertor))]
        public ObjectId Id { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public string AvatarUrl { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Data { get; set; }

        [JsonIgnore]
        public long GraphId { get; set; }

    }
}