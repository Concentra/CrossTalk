using System.Collections.Generic;
using Crosstalk.Core.Models;

namespace Crosstalk.Core.Repositories
{
    public interface IEdgeRepository
    {
        IEdgeRepository Save(Edge edge);
        Edge GetById(long id);
        IEnumerable<Edge> GetFromNode(Identity node);
        IEnumerable<Edge> GetToNode(Identity node);
        IEnumerable<Edge> GetToNode(Identity node, uint depth);
    }
}