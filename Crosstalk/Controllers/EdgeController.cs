using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages.Channels;
using Crosstalk.Core.Repositories;

namespace Crosstalk.Core.Controllers
{
    public class EdgeController : ApiController
    {
        // TODO: accept search depth on controller
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
            return this.Out(id, null);
        }
        
        [HttpGet]
        public IEnumerable<Edge> Out(string id, string type)
        {
            var edges = this._edgeRepository.GetFromNode(this._identityRepository.GetById(id), type);
            return this._identityRepository.BindPartials(edges, new string[] {"To", "From"});
        }

        [HttpGet]
        public IEnumerable<Edge> In(string id)
        {
            return this.In(id, null);
        }
        
        [HttpGet]
        public IEnumerable<Edge> In(string id, string type)
        {
            var edges = this._edgeRepository.GetToNode(this._identityRepository.GetById(id), type);
            return this._identityRepository.BindPartials(edges, new string[] {"To", "From"});
        }

        [HttpGet]
        public Edge Find(string from, string to, string type)
        {
            var fromId = this._identityRepository.GetById(from);
            var toId = this._identityRepository.GetById(to);

            return this._edgeRepository.GetByFromTo(fromId, toId, type);

        }
    }
}
