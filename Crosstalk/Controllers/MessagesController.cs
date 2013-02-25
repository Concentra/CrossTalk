using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Crosstalk.Core.Collections;
using Crosstalk.Core.Exceptions;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Services;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Controllers
{
    public class MessagesController : ApiController
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IEdgeRepository _edgeRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly IMessageService _messageService;

        public MessagesController(IMessageRepository messageRepository,
            IEdgeRepository edgeRepository,
            IIdentityRepository identityRepository,
            IMessageService messageService)
        {
            this._messageRepository = messageRepository;
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;
            this._messageService = messageService;
        }

        // GET api/messages?identity=id
        /// <summary>
        /// Gets all messages in the public space
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Message> GetByIdentity(string identity, bool thisIsMe)
        {
            var me = this._identityRepository.GetById(ObjectId.Parse(identity));
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, ChannelType.Public, (uint) (thisIsMe ? 2 : 1)));
            var messages = new List<Message>();
            foreach (var edge in edges)
            {
                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
                messages.AddRange(this._messageService.GetListForEdge(edge));
            }
            return messages;
        }

        [HttpGet]
        [ActionName("Feed")]
        public IEnumerable<Message> Feed(string id, IEnumerable<Edge> exclusions)
        {
            throw new Exception(string.Format("Endpoint deprecated use /api/feed/{0} instead", id));
            var me = this._identityRepository.GetById(id);
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, ChannelType.Public, 3))
                .Where(e => null == exclusions || !exclusions.Contains(e));
            var messages = new OrderedList<Message>((l, n) =>
                l.Created == n.Created ? 0 : l.Created > n.Created ? 1 : -1);
            Parallel.ForEach(edges, edge =>
                {
                    edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                    edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
                    lock (messages)
                    {
                        messages.AddRange(this._messageService.GetListForEdge(edge));
                    }
                });
            return messages;
        }

        [HttpGet]
        public IEnumerable<Message> Channel(string from, string to, string type)
        {
            return this.Channel(from, to, type, null);
        }

        [HttpGet]
        public IEnumerable<Message> Channel(string from, string to, string type, int? count)
        {
            var fId = this._identityRepository.GetById(from);
            var tId = this._identityRepository.GetById(to);

            var edges = new Edge[2]
                {
                    this._edgeRepository.GetByFromTo(fId, tId, type),
                    this._edgeRepository.GetByFromTo(tId, fId, type)
                };

            var messages = new OrderedList<Message>((l, n) =>
                l.Created == n.Created ? 0 : l.Created > n.Created ? 1 : -1);

            foreach (var edge in edges.Where(e => null != e))
            {
                edge.From = edge.From.Id == from ? fId : tId;
                edge.To = edge.To.Id == to ? tId : fId;
                
                foreach (var message in this._messageService.GetListForEdge(edge, count))
                {
                    messages.Add(message);
                }
            }

            return messages;
        }

        // GET api/messages/:id
        /// <summary>
        /// Get a specific message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public Message Get(string id)
        {
            return this._messageRepository.Get(id);
        }

        // POST api/messages
        public Message Post(Message message)
        {
            this._messageRepository.Save(message);
            return message;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            throw new OutsideOfProjectScopeException();
        }
    }
}