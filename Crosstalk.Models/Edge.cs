using MongoDB.Bson.Serialization.Attributes;

namespace Crosstalk.Core.Models
{
    public class Edge
    {
        public long Id { get; set; }
        [BsonIgnore]
        public Identity From;
        [BsonIgnore]
        public Identity To;
        [BsonIgnore]
        public bool IsPartial
        {
            get { return null == this.From && null == this.To; }
        } 
    }
}