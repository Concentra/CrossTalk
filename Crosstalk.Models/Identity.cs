using System;
using System.Collections.Generic;
using System.Linq;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Models
{
    public class Identity : IIdentity<Dictionary<string, object>>
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
            get {
                ObjectId oid;
                return ObjectId.TryParse(this.Id, out oid) ? oid : ObjectId.Empty;
            }
            set { this.Id = value.ToString(); }
        }

        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Type { get; set; }        

        [BsonIgnore]
        public Dictionary<string, object> Data { get; set; }

        //get
        //    {
        //        return null == this.Others ? null : this.Others.ToDictionary();
        //    }
        //    set
        //    {
        //        this.Others = new BsonDocument();
        //        foreach (var kv in value)
        //        {
        //            if (kv.Value is JArray)
        //            {
        //                var inner = new BsonArray();
        //                foreach (var item in (kv.Value as JArray))
        //                {
        //                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
        //                    var target = new BsonDocument(dict);
        //                    inner.Add(target);
        //                }
        //                this.Others.Add(kv.Key, inner);
        //            }   
        //        }
        //    }
        //}

        [JsonIgnore]
        public long GraphId { get; set; }

        public GraphIdentity ToGraphIdentity()
        {
            return new GraphIdentity
                {
                    Id = this.Id,
                    Type = this.Type
                };
        }

        [BsonExtraElements]
        [JsonIgnore]
        public BsonDocument Others { get; set; }

        public BsonDocument GetDataAsDocument()
        {
            var result = new BsonDocument();
            foreach (var kv in this.Data)
            {
                if (kv.Value is JArray)
                {
                    var inner = new BsonArray();
                    foreach (var item in (kv.Value as JArray))
                    {
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                        var target = new BsonDocument(dict);
                        inner.Add(target);
                    }
                    result.Add(kv.Key, inner);
                }
            }
            return result;
        }

    }
}