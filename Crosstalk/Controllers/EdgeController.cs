using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Repositories;
using System;
using System.Threading.Tasks;
using Crosstalk.Core.Models.Relationships;
using System.Linq;
using Newtonsoft.Json.Linq;

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

        [HttpPost]
        [ActionName("Delete")]
        public void Delete(JObject obj)
        {
            var fromId = obj.GetValue("FromId").ToObject<string>();
            var toId = obj.GetValue("ToId").ToObject<string>();
            var edge = this._edgeRepository.GetByFromTo(this._identityRepository.GetById(fromId), this._identityRepository.GetById(toId), ChannelType.Request);
            this._edgeRepository.Delete(edge);
        }

        [HttpPost]
        public void Save(Edge edge)
        {
            var actions = new List<Action>();

            if (0 == edge.From.GraphId)
            {
                actions.Add(() =>
                {
                    edge.From = this._identityRepository.GetById(edge.From.Id);
                });
            }
            if (0 == edge.To.GraphId)
            {
                actions.Add(() =>
                {
                    edge.To = this._identityRepository.GetById(edge.To.Id);
                });
            }
            Parallel.Invoke(actions.ToArray());
            this._edgeRepository.Save(edge);
        }

        [HttpGet]
        public IEnumerable<Edge> Both(string id)
        {
            var edges = this._edgeRepository.GetAllNode(this._identityRepository.GetById(id));
            return this._identityRepository.BindPartials<Edge>(edges, new [] { "To", "From" });
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
            return this._identityRepository.BindPartials(edges, new [] {"To", "From"});
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
            var toId = "network" == to ? fromId : this._identityRepository.GetById(to);

            return this._edgeRepository.GetByFromTo(fromId, toId, type);

        }
    }
}
