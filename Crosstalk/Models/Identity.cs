using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Crosstalk.Models
{
    public class Identity
    {
        public enum Types
        {
            Individual,
            Organisation,
            Broadcaster
        };

        public ObjectId Id;
        public string Name;
        public string AvatarUrl;
        public string OpenId;
        public Types Type;
    }
}