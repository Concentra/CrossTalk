using Crosstalk.Core.Enums;
using Crosstalk.Core.Models;
using Neo4jClient;

namespace Crosstalk.Core.Relationships
{
    public class Broadcast :
        Relationship,
        IRelationshipAllowingSourceNode<GraphIdentity>,
        IRelationshipAllowingTargetNode<GraphIdentity>
    {
        public Broadcast(NodeReference targetNode)
            : base(targetNode)
        {
        }

        public const string TypeKey = Edges.Broadcast;
        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
}