using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Crosstalk.Core.Enums;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Models.Relationships;
using MongoDB.Bson;
using Neo4jClient;
using Neo4jClient.Gremlin;

namespace Crosstalk.Core.Repositories
{
    public class EdgeRepository : BaseNeo4JRepository, IEdgeRepository
    {
        public const string Broadcast = "broadcast";
        public const string Private = "private";

        public EdgeRepository(IGraphClient client) : base(client) {}

        public IEdgeRepository Save(Edge edge, ChannelType type)
        {
            edge.Type = type;
            return this.Save(edge);
        }

        public IEdgeRepository Save(Edge edge){
            if (null == edge.Type)
            {
                throw new ArgumentNullException("edge", "Edge has no channel type");
            }
            var constructorInfo = edge.Type.ToType().GetConstructor(new Type[] {typeof (NodeReference)});
            if (constructorInfo == null)
            {   
                throw new ArgumentOutOfRangeException("edge", "Not a valid channel type");
            }
            var channel = (BaseChannel) constructorInfo.Invoke(new object[] { (NodeReference) edge.To.GraphId });
            this.Client.CreateRelationship((NodeReference<GraphIdentity>) edge.From.GraphId, channel);
            return this;
        }

        public Edge GetById(long id)
        {
            var nodes = this.Client.ExecuteGetAllNodesGremlin<GraphIdentity>(
                "g.e(edge).bothV",
                new Dictionary<string, object>
                    {
                        { "edge", id }
                    }).ToList();
            
            return new Edge
                {
                    Id = id,
                    From = new Identity
                        {
                            Id = nodes.Last().Data.Id
                        },
                    To = new Identity
                        {
                            Id = nodes.First().Data.Id
                        }
                };
        }

        /// <summary>
        /// Get nodes we broadcast to
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Edge> GetFromNode(Identity node, ChannelType type)
        {
            type = type ?? ChannelType.Public;
            return this.Client
                       .Get<GraphIdentity>(node.GraphId)
                       .OutE(type)
                       .Select(n => new Edge()
                       {
                           To = new Identity
                               {
                                   OId = ObjectId.Parse(this.Client.Get<GraphIdentity>(n.EndNodeReference).Data.Id),
                                   GraphId = n.EndNodeReference.Id
                               },
                           From = node,
                           Id = n.Reference.Id,
                           Type = n.TypeKey
                       });
        }

        /// <summary>
        /// Get nodes we receive broadcasts from
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Edge> GetToNode(Identity node, ChannelType type)
        {
            return this.GetToNode(node, type, 0);
        }

        public IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint depth)
        {
            return this.GetToNode(node, type, depth, true, null);
        }

        public IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint? depth, bool excludePublic, IEnumerable<string> exclusions)
        {
            type = type ?? ChannelType.Public;
            depth = depth ?? (uint) (ChannelType.Public == type ? 3 : 1);

            var query = string.Format(
                "g.v(node).as('x').outE(channel).inV.loop('x'){{it.loops < depth{0}{1}}}.path().scatter.dedup.filter{{it.Id == null}}",
                null == exclusions
                    ? ""
                    :  string.Format("&& !(it.object.Id in [\"{0}\"])", string.Join("\",\"", exclusions)),
                excludePublic
                    ? " && it.object.Type != \"public\""
                    : "");

            var rels = this.Client.ExecuteGetAllRelationshipsGremlin<Edge>(query,
                new Dictionary<string, object>()
                    {
                        {"depth", depth},
                        {"node", node.GraphId},
                        {"channel", type.ToString()}
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
                    Id = n.Reference.Id,
                    Type = n.TypeKey
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

        public Edge GetByFromTo(Identity @from, Identity to, ChannelType type)
        {
            var rType = type.ToType();
            return this.Client.Get<GraphIdentity>(from.GraphId)
                       .OutE(type)
                       .Where(rel => rel.EndNodeReference == to.GraphId)
                       .Select(e => new Edge
                           {
                               From = from,
                               To = to,
                               Id = e.Reference.Id,
                               Type = e.TypeKey
                           }).FirstOrDefault();
        }
    }
}