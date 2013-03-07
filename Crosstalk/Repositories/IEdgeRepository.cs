using System.Collections.Generic;
using Crosstalk.Common;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Models.Relationships;

namespace Crosstalk.Core.Repositories
{
    public interface IEdgeRepository : IPartialResolver<Edge>
    {
        IEdgeRepository Save(Edge edge, ChannelType type);
        IEdgeRepository Save(Edge edge);
        IEdgeRepository Delete(Edge edge);
        IEdgeRepository Delete(long id);
        Edge GetById(long id);
        IEnumerable<CypherReturnModel> GetAllNode(Identity node);
        IEnumerable<Edge> GetFromNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint depth);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint? depth, bool excludePublic, IEnumerable<string> exclusions);
        Edge GetByFromTo(Identity from, Identity to, ChannelType type);
    }
}
