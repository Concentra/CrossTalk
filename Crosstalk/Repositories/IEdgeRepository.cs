using System.Collections.Generic;
using Crosstalk.Common;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages.Channels;

namespace Crosstalk.Core.Repositories
{
    public interface IEdgeRepository : IPartialResolver<Edge>
    {
        IEdgeRepository Save(Edge edge, ChannelType type);
        IEdgeRepository Save(Edge edge);
        Edge GetById(long id);
        IEnumerable<Edge> GetFromNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint depth);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint? depth, bool excludePublic, IEnumerable<string> exclusions);
        Edge GetByFromTo(Identity from, Identity to, ChannelType type);
    }
}
