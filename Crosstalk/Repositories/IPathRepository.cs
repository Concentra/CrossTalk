using System.Collections.Generic;
using Crosstalk.Core.Models;

namespace Crosstalk.Core.Repositories
{
    public interface IPathRepository
    {
        IEnumerable<IEnumerable<Identity>> GetShortestBetweenNodes(Identity node1, Identity node2);
    }
}