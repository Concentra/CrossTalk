using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;

namespace Crosstalk.Core.Controllers
{
    public class EdgeController : ApiController
    {
        private readonly IEdgeRepository _edgeRepository;
        private readonly IIdentityRepository _identityRepository;

        public EdgeController(IEdgeRepository edgeRepository, IIdentityRepository identityRepository)
        {
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;
        }

        [HttpGet]
        public IEnumerable<Edge> Out(string id)
        {
            var edges = this._edgeRepository.GetFromNode(this._identityRepository.GetById(id));
            return this._identityRepository.BindPartials(edges, new string[] {"To", "From"});
        }

        [HttpGet]
        public IEnumerable<Edge> In(string id)
        {
            var edges = this._edgeRepository.GetToNode(this._identityRepository.GetById(id));
            return this._identityRepository.BindPartials(edges, new string[] {"To", "From"});
        }
    }
}
