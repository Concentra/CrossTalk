using Neo4jClient;
using System.ComponentModel;

namespace Crosstalk.Core.Models.Relationships
{
    public abstract class BaseChannel :
        Relationship,
        IRelationshipAllowingSourceNode<GraphIdentity>,
        IRelationshipAllowingTargetNode<GraphIdentity>
    {
        //Testing
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public BaseChannel() : base(0) {}
        //Testing

        protected BaseChannel(NodeReference targetNode)
            : base(targetNode) {}

        public abstract string TypeKey { get; }


        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
    }
}