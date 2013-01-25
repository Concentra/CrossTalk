using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Models.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crosstalk.Models
{
    public class Identity
    {
        public const string Public = "public";
        public const string Group = "group";
        public const string Person = "person";

        /// <summary>
        /// The data we actual care about
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public ObjectId OId
        {
            get { return ObjectId.Parse(this.Id); }
            set { this.Id = value.ToString(); }
        }

        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Type { get; set; }

        public Dictionary<string, string> Data { get; set; }

        [JsonIgnore]
        public long GraphId { get; set; }

        public bool HasId()
        {
            return ObjectId.Parse(this.Id) != ObjectId.Empty;
        }

        public GraphIdentity ToGraphIdentity()
        {
            return new GraphIdentity
                {
                    Id = this.Id,
                    Type = this.Type
                };
        }

    }
}