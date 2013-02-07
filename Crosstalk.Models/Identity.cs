using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Crosstalk.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonReader = Newtonsoft.Json.JsonReader;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace Crosstalk.Core.Models
{
    [JsonConverter(typeof(IdentityConvertor))]
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

        [JsonIgnore]
        public long GraphId { get; set; }

        public GraphIdentity ToGraphIdentity()
        {
            return new GraphIdentity
                {
                    Id = this.Id,
                    Type = this.Type.ToString()
                };
        }

        [BsonExtraElements]
        [JsonIgnore]
        [IgnoreDataMember]
        public BsonDocument Others { get; set; }

        public BsonDocument GetDataAsDocument()
        {
            var result = new BsonDocument();
            if (null == this.Data)
            {
                return result;
            }
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
                else if (kv.Value is JValue)
                {
                    result.Add(kv.Key, BsonValue.Create(kv.Value as JValue));
                }
                else
                {
                    result.Add(kv.Key, (string) kv.Value);
                }
            }
            return result;
        }

        public Dictionary<string, object> GetDictionaryFromDocument()
        {
            var doc = this.Others ?? new BsonDocument();
            var result = new Dictionary<string, object>();
            foreach (var element in doc)
            {
                object obj;
                if (element.Value.IsBsonArray)
                {
                    obj = JArray.Parse(element.Value.ToJson());
                } else if (element.Value.IsBsonDocument)
                {
                    obj = BsonSerializer.Deserialize<Dictionary<string, string>>(element.Value.ToJson());
                }
                else
                {
                    obj = element.Value.RawValue;
                }
                result.Add(element.Name, obj);
            }
            return result;
        }
    }

    public class IdentityConvertor : JsonConverter
    {
        private bool _canWrite = true;

        public override bool CanWrite
        {
            get
            {
                return this._canWrite;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Identity))
            {
                throw new ArgumentException("value is not Identity", "value");
            }
            var target = (Identity) value;
            target.Data = target.GetDictionaryFromDocument();
            serializer.Converters.Remove(this);
            this._canWrite = false;
            serializer.Serialize(writer, target);
            this._canWrite = true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var target = new Identity();
            serializer.Populate(obj.CreateReader(), target);
            //target.Others = target.GetDataAsDocument();
            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Identity).IsAssignableFrom(objectType);
        }
    }
}