using System.Collections.Generic;
using System.Linq;
using Crosstalk.Core.Enums;
using Crosstalk.Core.Models;
using Crosstalk.Core.Relationships;
using MongoDB.Bson;
using Neo4jClient;
using Neo4jClient.Gremlin;

namespace Crosstalk.Core.Repositories
{
    public class EdgeRepository : BaseNeo4JRepository, IEdgeRepository
    {
        public const string Broadcast = "broadcast";
        public const string Private = "private";

        public EdgeRepository(GraphClient client) : base(client) {}

        public IEdgeRepository Save(Edge edge)
        {
            this.GetClient().CreateRelationship<GraphIdentity, Broadcast>((NodeReference<GraphIdentity>) edge.From.GraphId, new Broadcast((NodeReference) edge.To.GraphId));
            return this;
        }

        public Edge GetById(long id)
        {
            var nodes = this.GetClient().RootNode.InE(id.ToString()).BothV<GraphIdentity>().ToList();
            return new Edge()
                {
                    Id = id,
                    From = new Identity
                        {
                            Id = nodes[0].Data.Id
                        },
                    To = new Identity
                        {
                            Id = nodes[1].Data.Id
                        }
                };
        }

        public IEnumerable<Edge> GetFromNode(Identity node)
        {
            return this.GetClient()
                       .Get<GraphIdentity>(node.GraphId)
                       .OutE(Edges.Broadcast)
                       .Select(n => new Edge()
                       {
                           To = new Identity
                               {
                                   OId = ObjectId.Parse(this.Client.Get<GraphIdentity>(n.EndNodeReference).Data.Id),
                                   GraphId = n.EndNodeReference.Id
                               },
                           From = node,
                           Id = n.Reference.Id
                       });
        }

        public IEnumerable<Edge> GetToNode(Identity node)
        {
            return this.GetToNode(node, 1);
        }

        public IEnumerable<Edge> GetToNode(Identity node, uint depth)
        {
            // g.v(317).as('x').outE.as('edge').inV.loop('x'){it.loops < 3 && it.object.Type != "public"}.path().scatter.dedup.filter{it.Id == null}.id
            var rels = this.Client.ExecuteGetAllRelationshipsGremlin<Edge>(
                "g.v(node).as('x').outE.inV.loop('x'){it.loops < 3 && it.object.Type != type}.path().scatter.dedup.filter{it.Id == null}",
                new Dictionary<string, object>()
                    {
                        {"node", node.GraphId},
                        {"type", Identity.Public}
                    });
            return rels.Select(n => new Edge()
                {
                    From = new Identity
                        {
                            OId = ObjectId.Parse(this.Client.Get<GraphIdentity>(n.StartNodeReference).Data.Id),
                            GraphId = n.StartNodeReference.Id
                        },
                    To = new Identity
                        {
                            Id = this.Client.Get<GraphIdentity>(n.EndNodeReference).Data.Id,
                            GraphId = n.EndNodeReference.Id
                        },
                    Id = n.Reference.Id
                });
            return this.Client
                       .Get<GraphIdentity>(node.GraphId)
                       //.As<GraphIdentity>("x")
                       .InE(Edges.Broadcast)
                       //.LoopV<GraphIdentity>("x", depth)
                       //.Where(v => v.Data.Type == Identity.Public)
                       //.
                       .Select(n => new Edge()
                       {
                           From = new Identity {
                               OId = ObjectId.Parse(this.Client.Get<GraphIdentity>(n.StartNodeReference).Data.Id),
                               GraphId = n.StartNodeReference.Id
                           },
                           To = node,
                           Id = n.Reference.Id
                       });
        }
    }
}