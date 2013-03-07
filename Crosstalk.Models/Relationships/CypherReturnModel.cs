using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Core.Models.Relationships
{
    public class CypherReturnModel : 
        Relationship,
        IRelationshipAllowingSourceNode<Identity>,
        IRelationshipAllowingTargetNode<Identity>
    {
        public CypherReturnModel() : base(0) { }

        public CypherReturnModel(NodeReference sourceNode, NodeReference targetNode, string typeKey, long id, object data) : base(targetNode, data)
        {
            this.start = sourceNode;
            this.end = targetNode;
            this.TypeKey = typeKey;
            this.Id = id;
        }

        public NodeReference start { get; set; }
        public NodeReference end { get; set; }
        public string TypeKey { get; set; }
        public long Id { get; set; }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
}
