using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonReader = Newtonsoft.Json.JsonReader;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace Crosstalk.Core.Models
{
    //[BsonSerializer(typeof(IdentitySerializer))]
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
                    obj = JObject.Parse(element.Value.ToJson());
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
            target.Others = target.GetDataAsDocument();
            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Identity).IsAssignableFrom(objectType);
        }
    }

    //public class IdentitySerializer : DictionarySerializer, IBsonDocumentSerializer, IBsonIdProvider
    //{
    //    public new object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public new object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
    //    {
    //        if (typeof (Identity) != nominalType)
    //        {
    //            throw new ArgumentException(string.Format("{0} is not a valid Identity type", nominalType.ToString()), "nominalType");
    //        }
    //        var doc = BsonSerializer.Deserialize<IDictionary<string, object>>(bsonReader);
    //        var result = new Identity();
    //        foreach (var kv in doc)
    //        {
    //            var field = nominalType.GetField(kv.Key);
    //            if (null != field)
    //            {
    //                field.SetValue(result, kv.Value);
    //            }
    //            else
    //            {
    //                result.Data[kv.Key] = kv.Value;
    //            }
    //        }
    //        return result;
    //    }

    //    public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
    //    {
    //        if (value.GetType() != nominalType || typeof(Identity) != nominalType)
    //        {
    //            throw new ArgumentException("Invalid argument types");
    //        }
    //        (value as Identity).Others = (value as Identity).GetDataAsDocument();
    //        base.Serialize(bsonWriter, nominalType, value, options);
    //    }

    //    public BsonSerializationInfo GetMemberSerializationInfo(string memberName)
    //    {
    //        var field = typeof (Identity).GetField(memberName);
    //        if (field == null)
    //        {
    //            return new BsonSerializationInfo(memberName, this, typeof(object), new DictionarySerializationOptions());
    //        }
    //        var representations =
    //            field.GetCustomAttributes(typeof (BsonRepresentationAttribute), false)
    //                 .Cast<BsonRepresentationAttribute>();
    //        IBsonSerializationOptions opt;
    //        var representationAttrs = representations as IList<BsonRepresentationAttribute> ?? representations.ToList();
    //        if (representationAttrs.Any())
    //        {
    //            opt = new RepresentationSerializationOptions(representationAttrs.First().Representation);
    //        }
    //        else
    //        {
    //            opt = new RepresentationSerializationOptions(BsonType.String);
    //        }
    //        return new BsonSerializationInfo(memberName, this, field.GetType(), opt);
    //    }

    //    public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
    //    {
    //        id = (document as Identity).OId;
    //        idNominalType = typeof (ObjectId);
    //        idGenerator = new ObjectIdGenerator();
    //        return true;
    //    }

    //    public void SetDocumentId(object document, object id)
    //    {
    //        (document as Identity).OId = (ObjectId) id;
    //    }
    //}
}