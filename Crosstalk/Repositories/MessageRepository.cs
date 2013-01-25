using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using Crosstalk.Models;

namespace Crosstalk.Repositories
{
    public class MessageRepository : BaseMongoRepository<Message>, IMessageRepository
    {
        public MessageRepository(MongoDatabase database) : base(database) {}

        protected override string Collection
        {
            get { return "messages"; }
        }

        public IList<Message> GetList()
        {
            return this.GetCollection().FindAll().ToList();
        }

        public IList<Message> GetListForEdge(Edge edge)
        {
            return this.GetCollection().AsQueryable().Where(m => m.Edge.Id == edge.Id).Select(m => new Message
                {
                    Body = m.Body,
                    Id = m.Id,
                    Edge = edge
                }).ToList();
        }

        public Message Get(string messageId)
        {
            return this.GetCollection().AsQueryable().Single(m => m.Id == ObjectId.Parse(messageId));
        }

        public bool Save(Message message)
        {
            var result = this.GetCollection().Insert(message);
            return result.Ok;
        }
    }
}