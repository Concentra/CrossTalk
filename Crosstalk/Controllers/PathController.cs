using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Repositories;
using System.Collections.Generic;

namespace Crosstalk.Core.Controllers
{
    public class PathController : ApiController
    {
        private readonly IPathRepository _pathRepository;
        private readonly IIdentityRepository _identityRepository;

        public PathController(IPathRepository pathRepository, IIdentityRepository identityRepository)
        {
            this._pathRepository = pathRepository;
            this._identityRepository = identityRepository;
        }

        [HttpGet]
        public IEnumerable<IEnumerable<Identity>> Shortest(string id1, string id2)
        {
            var from = this._identityRepository.GetById(id1);
            var to = this._identityRepository.GetById(id2);
            return this._pathRepository.GetShortestBetweenNodes(from, to);
        }
    }
}
