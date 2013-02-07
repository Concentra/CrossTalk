using System.Collections.Generic;
using System.Linq;
using Crosstalk.Common.Repositories;
using Crosstalk.Core.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Crosstalk.Core.Repositories
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
            return this.GetListForEdge(edge, null);
        }

        public IList<Message> GetListForEdge(Edge edge, int? count)
        {
            IQueryable<Message> results = this.GetCollection().AsQueryable()
                .Where(m => m.Edge.Id == edge.Id)
                .OrderByDescending(m => m.Created);
            
            if (count.HasValue)
            {
                results = results.Take(count.Value);
            }

            return results.Select(m => new Message
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