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
        //public void Delete(Edge edge)
        public void Delete(JObject obj)
        {
            var edgeId = obj.GetValue("EdgeId").ToObject<long>();
            //var edgeId = edge.Id;
            this._edgeRepository.Delete(edgeId);
        }

        [HttpPost]
        [ActionName("Accept")]
        //public void Accept(Edge edge)
        public void Accept(JObject obj)
        {
            var edgeId = obj.GetValue("EdgeId").ToObject<long>();
            //var edgeId = edge.Id;
            var originalEdge = this._edgeRepository.GetById(edgeId);
            var forwardEdge = new Edge() { 
                From = originalEdge.From,
                To = originalEdge.To,
                cType = ChannelType.Public };
            var reverseEdge = new Edge()
            {
                From = originalEdge.To,
                To = originalEdge.From,
                cType = ChannelType.Public
            };
            this._edgeRepository.Save(forwardEdge);
            this._edgeRepository.Save(reverseEdge);
            this._edgeRepository.Delete(edgeId);
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
            var cRM = this._edgeRepository.GetAllNode(this._identityRepository.GetById(id));
            
            var edges = cRM.Select(c =>
                new Edge()
                {
                    Id = c.Id,
                    cType = (ChannelType)c.RelationshipTypeKey,
                    From = this._identityRepository.GetByGraphId(c.start.Id),
                    To = this._identityRepository.GetByGraphId(c.end.Id)
                });

            return edges;
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
