using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Enums;
using Crosstalk.Models;
using Crosstalk.Relationships;
using Neo4jClient;
using Neo4jClient.Gremlin;

namespace Crosstalk.Repositories
{
    public class EdgeRepository : BaseNeo4JRepository, IEdgeRepository
    {
        public const string Broadcast = "broadcast";
        public const string Private = "private";

        public EdgeRepository(GraphClient client) : base(client) {}

        public IEdgeRepository Save(Edge edge)
        {
            this.GetClient().CreateRelationship<Identity, Broadcast>((NodeReference<Identity>) edge.From.GraphId, new Broadcast((NodeReference) edge.To.GraphId));
            return this;
        }

        public Edge GetById(long id)
        {
            var nodes = this.GetClient().RootNode.InE(id.ToString()).BothV<Crosstalk.Models.Identity>().ToList();
            return new Edge()
                {
                    Id = id,
                    From = nodes[0].Data,
                    To = nodes[1].Data
                };
        }

        public IEnumerable<Edge> GetFromNode(Identity node)
        {
            return this.GetClient()
                .RootNode
                .In<Identity>(Identities.Label, i => i.Id == node.Id)
                .OutE(Edges.Broadcast)
                .Select(n => new Edge()
                    {
                        To = this.GetClient().Get<Identity>(n.StartNodeReference).Data,
                        From = node,
                        Id = n.Reference.Id
                    });
        }

        public IEnumerable<Edge> GetToNode(Identity node)
        {
            return this.GetClient()
                .RootNode
                .In<Identity>(Identities.Label, i => i.Id == node.Id)
                .InE(Edges.Broadcast)
                .Select(n => new Edge()
                {
                    From = this.GetClient().Get<Identity>(n.StartNodeReference).Data,
                    To = node,
                    Id = n.Reference.Id
                });
        }
    }
}