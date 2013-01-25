using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Exceptions;
using Crosstalk.Models;
using Crosstalk.Repositories;
using MongoDB.Bson;

namespace Crosstalk.Controllers
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
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, (uint) (thisIsMe ? 2 : 1)));
            var messages = new List<Message>();
            foreach (var edge in edges)
            {
                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
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