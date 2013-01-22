using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Enums;
using Crosstalk.Models;
using Neo4jClient;

namespace Crosstalk.Relationships
{
    public class Broadcast :
        Relationship,
        IRelationshipAllowingSourceNode<Identity>,
        IRelationshipAllowingTargetNode<Identity>
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