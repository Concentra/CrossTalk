using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;

namespace Crosstalk.Core.Services
{
    public class EdgeService : IEdgeService
    {
        private readonly IEdgeRepository _edgeRepository;
        private readonly IIdentityRepository _identityRepository;

        public EdgeService(IEdgeRepository edgeRepository, IIdentityRepository identityRepository)
        {
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;
        }

        public Edge GetById(string id)
        {
            var edge = this._edgeRepository.GetById(id);
            Parallel.Invoke(() => edge.From = this._identityRepository.GetById(edge.From.Id),
                            () => edge.To = this._identityRepository.GetById(edge.To.Id));
            return edge;
        }
    }
}