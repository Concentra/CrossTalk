using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Crosstalk.Binders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Crosstalk.Models
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Edge Edge { get; set; }
        public string Body { get; set; }
    }
}