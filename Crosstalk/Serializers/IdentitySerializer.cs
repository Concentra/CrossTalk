//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Crosstalk.Core.Models;
//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using MongoDB.Bson.Serialization;

//namespace Crosstalk.Core.Serializers
//{
//    public class IdentitySerializer : MongoDB.Bson.Serialization.BsonClassMapSerializer, IBsonSerializer
//    {
//        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
//        {
//            throw new NotImplementedException();
//        }

//        public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
//        {
//            throw new NotImplementedException();
//        }

//        public IBsonSerializationOptions GetDefaultSerializationOptions()
//        {
//            throw new NotImplementedException();
//        }

//        public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
//        {
//            if (nominalType != typeof (Identity))
//            {
//                throw new ArgumentException("nominalType is not of type Identity");
//            }
//            var result = new BsonDocument();
//            foreach (var prop in typeof (Identity).GetFields().Where(i => "Data" != i.Name))
//            {
//                result.Add(prop.Name, prop.Attributes)
//            }
//        }
//    }
//}