using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using MongoDB.Bson;

namespace Crosstalk.Models
{
    public class Message
    {
        private ObjectId _edge;

        public ObjectId Id { get; set; }
        public ObjectId Edge { get; set; }
        public string Body { get; set; }
    }
}