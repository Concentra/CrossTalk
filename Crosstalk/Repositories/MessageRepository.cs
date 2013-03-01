using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Crosstalk.Common.Repositories;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using Crosstalk.Common.Models;

namespace Crosstalk.Core.Repositories
{
    public class MessageRepository : BaseMongoRepository<Message>, IMessageRepository
    {
        public MessageRepository(MongoDatabase database) : base(database) {}

        public IEnumerable<Message> Search(NameValueCollection parameters)
        {
            var queries = new List<IMongoQuery>();

            foreach (var key in parameters.AllKeys)
            {
                var vals = parameters.GetValues(key);
                if (null == vals) continue;
                if (vals.Count() > 1)
                {
                    queries.Add(Query.In(key, vals.Select(BsonValue.Create)));
                }
                else if (vals.Count() == 1)
                {
                    queries.Add(Query.EQ(key, BsonValue.Create(vals.First())));
                }
            }

            return this.GetCollection().Find(Query.And(queries));
        }

        public void Moderate(string id)
        {
            var msg = this.GetById(id);
            msg.Status = ReportableStatus.Removed;
            this.Save(msg);
        }

        public void Revoke(string id)
        {
            var msg = this.GetById(id);
            msg.Status = ReportableStatus.Missing;
            this.Save(msg);
        }

        public void Report(string id)
        {
            var msg = this.GetById(id);
            msg.Status = ReportableStatus.Reported;
            this.Save(msg);
        }

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

            return results.ToList();
        }

        public Message GetById(string messageId)
        {
            return this.GetCollection().AsQueryable().SingleOrDefault(m => m.Id == messageId);
        }

        public bool Save(Message message)
        {
            message.Id = null == message.Id || ObjectId.Empty.ToString().Equals(message.Id)
                ? ObjectId.GenerateNewId().ToString()
                : message.Id;
            message.Created = DateTime.Now;
            if (null != message.OriginalMessage)
            {
                message.OriginalMessageId = message.OriginalMessage.Id;
            }
            return (this.GetCollection().AsQueryable().Any(m => m.Id == message.Id)
                ? this.GetCollection().Save(message)
                : this.GetCollection().Insert(message)).Ok;
        }

        public long CountShares(string messageId)
        {
            var query = Query.EQ("OriginalMessageId", ObjectId.Parse(messageId));
            return this.GetCollection().Find(query).Count();
        }

        public int Count(Func<Message, bool> predicate)
        {
            return this.GetCollection().AsQueryable().Count(predicate);
        }
    }
}