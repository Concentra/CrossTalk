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
    public class MessageRepository : MongoRepository<Message>, IMessageRepository
    {
        public MessageRepository(MongoDatabase database) : base(database) {}

        protected override string GetCollectionName()
        {
            return "messages";
        }

        public IList<Message> GetList()
        {
            return this.GetCollection().FindAll().ToList();
        }

        public IList<Message> GetListForEdge(Edge edge)
        {
            throw new NotImplementedException();
        }

        public Message Get(string messageId)
        {
            return this.GetCollection().FindOne(Query.EQ("_id", messageId));
        }

        public bool Save(Message message)
        {
            var result = this.GetCollection().Insert(message);
            return result.Ok;
        }
    }
}