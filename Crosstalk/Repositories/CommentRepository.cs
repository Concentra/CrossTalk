using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Crosstalk.Core.Models.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Crosstalk.Common.Repositories;
using MongoDB.Driver.Builders;
using Crosstalk.Common.Models;

namespace Crosstalk.Core.Repositories
{
    public class CommentRepository : BaseMongoRepository<Message>, ICommentRepository
    {
        private readonly IMessageRepository _messageRepository;

        public CommentRepository(MongoDatabase db, IMessageRepository messageRepository) : base(db)
        {
            this._messageRepository = messageRepository;
        }

        protected override string Collection
        {
            get
            {
                return "messages";
            }
        }

        public void Save(Comment comment)
        {
            if (null == comment.ParentMessage)
            {
                throw new Exception("ParentMessage is required to save comment");
            }
            var message = comment.ParentMessage.Value;
            comment.Created = DateTime.Now;
            var comments = null == message.Comments
                ? new List<Comment>()
                : (message.Comments as List<Comment>
                    ?? message.Comments.ToList<Comment>());
            if (null == comment.Id || ObjectId.Empty.ToString().Equals(comment.Id))
            {
                comment.Id = ObjectId.GenerateNewId().ToString();
            }
            else
            {
                comments.RemoveAll(c => c.Id == comment.Id);
            }
            comments.Add(comment);
            message.Comments = comments;
            this._messageRepository.Save(message);
        }

        public void Delete(Comment comment)
        {
            if (null == comment.ParentMessage)
            {
                throw new Exception("ParentMessage is required to save comment");
            }
            var message = comment.ParentMessage.Value;
            if (null == message.Comments)
            {
                // Nothing to do
                return;
            }
            var comments = null == message.Comments
                ? new List<Comment>()
                : (message.Comments as List<Comment>
                    ?? message.Comments.ToList<Comment>());
            comments.RemoveAll(c => c.Id == comment.Id);
            comment.Status = ReportableStatus.Revoked;
            comments.Add(comment);
            message.Comments = comments;
            this._messageRepository.Save(message);
        }

        public IEnumerable<Comment> Search(NameValueCollection parameters)
        {
            var queries = new List<IMongoQuery>();
            var refiningQueries = new List<IMongoQuery>();

            foreach (var key in parameters.AllKeys)
            {
                var vals = parameters.GetValues(key);
                if (null == vals) continue;
                if (vals.Count() > 1)
                {
                    queries.Add(Query.In(key, vals.Select(BsonValue.Create)));
                    refiningQueries.Add(Query.In("Comments." + key, vals.Select(BsonValue.Create)));
                }
                else if (vals.Count() == 1)
                {
                    queries.Add(Query.EQ(key, BsonValue.Create(vals.First())));
                    refiningQueries.Add(Query.EQ("Comments." + key, BsonValue.Create(vals.First())));
                }
            }

            var ops = new [] {
                new BsonDocument
                    {
                        {
                            "$match",
                            Query.ElemMatch("Comments", Query.And(queries)).ToBsonDocument()
                        }
                    },
                new BsonDocument
                    {
                        {
                            "$project",
                            new BsonDocument
                                {
                                    { "Comments", 1 },
                                    { "_id", 0 }
                                }
                        }
                    },
                new BsonDocument
                    {
                        {
                            "$unwind", "$Comments"
                        }
                    },
                new BsonDocument
                    {
                        {
                            "$match",
                            Query.And(refiningQueries).ToBsonDocument()
                        }
                    }
            };

            var result = this.GetCollection().Aggregate(ops);
            foreach (var comment in result.ResultDocuments)
            {
                yield return BsonSerializer.Deserialize<Comment>(comment.GetValue("Comments").AsBsonDocument);
            }
        }

        public Comment GetById(string id)
        {
            return this.Search(new NameValueCollection {
                { "_id", id }
            }).FirstOrDefault();
        }

        public void Report(string id)
        {
            var comments = this.Search(new NameValueCollection {
                {"_id", id}
            });
            if (comments.Count() != 1)
            {
                throw new Exception("Multiple comments found with that ID");
            }
            var msg = this._messageRepository.GetById(comments.First().ParentMessage.Id);
            msg.Comments = msg.Comments.Select(c => {
                if (c.Id == id)
                {
                    c.Status = ReportableStatus.Reported;
                }
                return c;
            });
            this._messageRepository.Save(msg);
        }

        public void Moderate(string id)
        {
            var comments = this.Search(new NameValueCollection {
                {"_id", id}
            });
            if (comments.Count() != 1)
            {
                throw new Exception("Multiple comments found with that ID");
            }
            var msg = this._messageRepository.GetById(comments.First().ParentMessage.Id);
            msg.Comments = msg.Comments.Select(c =>
            {
                if (c.Id == id)
                {
                    c.Status = ReportableStatus.Removed;
                }
                return c;
            });
            this._messageRepository.Save(msg);
        }

        public void Revoke(string id)
        {

            var comments = this.Search(new NameValueCollection {
                {"_id", id}
            });
            if (comments.Count() != 1)
            {
                throw new Exception("Multiple comments found with that ID");
            }
            var msg = this._messageRepository.GetById(comments.First().ParentMessage.Id);
            msg.Comments = msg.Comments.Select(c =>
            {
                if (c.Id == id)
                {
                    c.Status = ReportableStatus.Revoked;
                }
                return c;
            });
            this._messageRepository.Save(msg);
        }
    }
}