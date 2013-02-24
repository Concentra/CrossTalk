using System;
using System.Collections.Generic;
using System.Linq;
using Crosstalk.Core.Enums;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Models.Relationships;
using MongoDB.Bson;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Crosstalk.Core.Repositories
{
    public class PathRepository : BaseNeo4JRepository, IPathRepository
    {
        public PathRepository(IGraphClient client) : base(client) {}

        public IEnumerable<IEnumerable<Identity>> GetShortestBetweenNodes(Identity node1, Identity node2)
        {
            //TODO: Change to a working Gremlin query - problem is 'public' node
            //Alternative: Either connections between identities and the public node are always directed towards public
            //Or: Change edge type between everyone and public node.

            return new CypherFluentQuery(this.Client)
                .Start("n", (NodeReference)node1.GraphId)
                .AddStartPoint("m", (NodeReference)node2.GraphId)
                .Match("p=allShortestPaths(n-[:broadcast*..20]-m)")
                .Return<IEnumerable<Identity>>("nodes(p)")
                .Results;
        }
    }
}