using System.Collections.Generic;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;

namespace Crosstalk.Core.Repositories
{
    public interface IEdgeRepository
    {
        IEdgeRepository Save(Edge edge, ChannelType type);
        //Edge GetById(long id);
        IEnumerable<Edge> GetFromNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type);
        IEnumerable<Edge> GetToNode(Identity node, ChannelType type, uint depth);
        Edge GetByFromTo(Identity from, Identity to, ChannelType type);
    }
}