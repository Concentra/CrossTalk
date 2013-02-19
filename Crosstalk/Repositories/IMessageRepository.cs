using System;
using System.Collections.Generic;
using Crosstalk.Core.Models;
using MongoDB.Bson;

namespace Crosstalk.Core.Repositories
{
    public interface IMessageRepository
    {
        IList<Message> GetList();
        IList<Message> GetListForEdge(Edge edge);
        IList<Message> GetListForEdge(Edge edge, int? count);
        Message Get(string messageId);
        bool Save(Message message);
        long CountShares(string messageId);
        int Count(Func<Message, bool> predicate);
        // void Moderate(int messageId);
        // void Report(int messageId);
    }
}
