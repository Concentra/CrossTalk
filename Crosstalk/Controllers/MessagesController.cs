using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Crosstalk.Binders;
using Crosstalk.Models;
using Crosstalk.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

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
        //public string Post(JObject post)
        public Message Post(Message message)
        {
            //var message = new Message()
            //    {
            //        Edge = post["Edge"].ToString(),
            //        Body = post["Body"].ToString()
            //    };
            message.Id = ObjectId.GenerateNewId().ToString();
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
        }
    }
}