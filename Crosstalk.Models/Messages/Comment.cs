using System;
using System.Collections.Generic;
using System.Linq;
using Crosstalk.Common.Models;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Crosstalk.Core.Models.Messages.Messages
{
    public class Comment : IComment
    {
        public Partial<IIdentity> Author { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public ReportableStatus Status { get; set; }
    }
    /*public class IIdentityPartialSerializer : BsonBaseSerializer
    {
        private void AssertNominalType(Type nominalType)
        {
            if (nominalType != typeof(IIdentity)) throw new ArgumentException("nominalType is not an IIdentity");
        }
        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            this.AssertNominalType(nominalType);
            var ser = new DictionarySerializer<string, string>();
            var res = (Dictionary<string, string>) ser.Deserialize(bsonReader, typeof(Dictionary<string, string>), options);
            return new Identity
            {
                Id = res["Id"]
            };
        }
        public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            this.AssertNominalType(nominalType);
            var partial = new Dictionary<string, string> {
                { "Id", ((IIdentity) value).Id }
            };
            var ser = new DictionarySerializer<string, string>();
            ser.Serialize(bsonWriter, nominalType, partial, options);
        }
    }*/
}
