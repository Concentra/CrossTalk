using System;
using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Core.Exceptions;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;

namespace Crosstalk.Core.Controllers
{
    public class MessagesController : ApiController
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IEdgeRepository _edgeRepository;
        private readonly IIdentityRepository _identityRepository;

        public MessagesController(IMessageRepository messageRepository, IEdgeRepository edgeRepository, IIdentityRepository identityRepository)
        {
            this._messageRepository = messageRepository;
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;
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
                messages.AddRange(this._messageRepository.GetListForEdge(edge));
            }
            return messages;
        }

        [HttpGet]
        public IEnumerable<Message> Feed(string id)
        {
            var me = this._identityRepository.GetById(id);
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, ChannelType.Public, 2));
            var messages = new List<Message>();
            foreach (var edge in edges)
            {
                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
                messages.AddRange(this._messageRepository.GetListForEdge(edge));
            }
            return messages;
        }

        [HttpGet]
        public IEnumerable<Message> GetByChannel(string from, string to, ChannelType type)
        {
            var fId = this._identityRepository.GetById(from);
            var tId = this._identityRepository.GetById(to);

            var edges = new Edge[2]
                {
                    this._edgeRepository.GetByFromTo(fId, tId, type),
                    this._edgeRepository.GetByFromTo(tId, fId, type)
                };

            var messages = new List<Message>();

            foreach (var edge in edges)
            {
                edge.From = edge.From.Id == from ? fId : tId;
                edge.To = edge.To.Id == to ? tId : fId;
                messages.AddRange(this._messageRepository.GetListForEdge(edge));
            }

            return messages;
        }

        // GET api/messages/:id
        /// <summary>
        /// Get a specific message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Message Get(string id)
        {
            return this._messageRepository.Get(id);
        }

        // POST api/messages
        public Message Post(Message message)
        {
            message.Id = ObjectId.GenerateNewId();
            message.Created = DateTime.Now;
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