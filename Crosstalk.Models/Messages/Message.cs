using System;
using System.Collections.Generic;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models.Messages.Convertors;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Crosstalk.Common.Convertors;
using System.ComponentModel;

namespace Crosstalk.Core.Models.Messages
{
    [BsonIgnoreExtraElements]
    public class Message : IReportable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Edge Edge { get; set; }

        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OriginalMessageId { get; set; }

        [BsonIgnore]
        public Message OriginalMessage { get; set; }
        
        public String Body { get; set; }
        
        public DateTime Created { get; set; }

        public bool Read { get; set; }

        public int NumberOfShares { get; set; }

        public IEnumerable<Comment> Comments { get; set; }

        public IEnumerable<Report> Reports { get; set; }

        [DefaultValue(typeof(ReportableStatus), "none")]
        [JsonConverter(typeof(ReportableStatusConvertor))]
        public ReportableStatus Status { get; set; }
    }
}