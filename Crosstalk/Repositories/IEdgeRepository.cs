using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Models;

namespace Crosstalk.Repositories
{
    public interface IEdgeRepository
    {
        IEdgeRepository Save(Edge edge);
        Edge GetById(long id);
        IEnumerable<Edge> GetFromNode(Identity node);
        IEnumerable<Edge> GetToNode(Identity node);
    }
}