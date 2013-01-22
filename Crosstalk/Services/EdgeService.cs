using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Repositories;

namespace Crosstalk.Services
{
    public class EdgeService
    {
        private readonly IEdgeRepository _edgeRepository;

        public EdgeService(IEdgeRepository edgeRepository)
        {
            this._edgeRepository = edgeRepository;
        }
    }
}