using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crosstalk.Models;
using Crosstalk.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace Crosstalk.Controllers
{
    public class MessagesController : ApiController
    {

        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMessageRepository repository)
        {
            this._messageRepository = repository;
        }

        // GET api/values
        /// <summary>
        /// Gets all messages in the public space
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Message> Get()
        {
            return this._messageRepository.GetList();
        }

        // GET api/values/5
        /// <summary>
        /// Get a specific message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "";
        }

        // POST api/values
        public string Post([FromBody] Message message)
        {
            var id = this._messageRepository.Save(message);
            return ObjectId.Empty.Equals(id) ? null : ((ObjectId) id).ToString();
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}