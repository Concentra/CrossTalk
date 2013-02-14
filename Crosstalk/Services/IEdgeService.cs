using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosstalk.Core.Models;

namespace Crosstalk.Core.Services
{
    public interface IEdgeService
    {
        /// <summary>
        /// Gets an edge and binds partials.
        /// </summary>
        /// <param name="id">The id of the edge</param>
        /// <returns>The Edge identified by the id</returns>
        Edge GetById(long id);
    }
}
