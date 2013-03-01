using Neo4jClient;

namespace Crosstalk.Core.Models.Messages.Relationships
{
    public abstract class BaseChannel :
        Relationship,
        IRelationshipAllowingSourceNode<GraphIdentity>,
        IRelationshipAllowingTargetNode<GraphIdentity>
    {
        protected BaseChannel(NodeReference targetNode)
            : base(targetNode) {}

        public abstract string TypeKey { get; }


        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
}