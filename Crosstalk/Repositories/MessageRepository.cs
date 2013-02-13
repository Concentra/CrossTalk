using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crosstalk.Common.Repositories;
using Crosstalk.Core.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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

            var list = results.ToList();

            Parallel.ForEach(list,
                             m =>
                                {
                                    if (ObjectId.Empty != m.OriginalMessageId)
                                    {
                                        m.OriginalMessage = this.Get(m.OriginalMessageId.ToString());
                                    }
                                    m.NumberOfShares =
                                        this.GetCollection().AsQueryable().Count(s => s.OriginalMessageId == m.Id);
                                    m.Edge = edge;
                                });

            return list;
        }

        public Message Get(string messageId)
        {
            return this.GetCollection().AsQueryable().Single(m => m.Id == ObjectId.Parse(messageId));
        }

        public bool Save(Message message)
        {
            return (this.GetCollection().AsQueryable().Any(m => m.Id == message.Id)
                ? this.GetCollection().Save(message)
                : this.GetCollection().Insert(message)).Ok;
        }
    }
}