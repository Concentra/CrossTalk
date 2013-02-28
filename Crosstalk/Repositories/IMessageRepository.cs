using System;
using System.Collections.Generic;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages;
using Crosstalk.Common;

namespace Crosstalk.Core.Repositories
{
    public interface IMessageRepository : IPartialResolver<Message>
    {
        IList<Message> GetList();
        IList<Message> GetListForEdge(Edge edge);
        IList<Message> GetListForEdge(Edge edge, int? count);
        bool Save(Message message);
        long CountShares(string messageId);
        int Count(Func<Message, bool> predicate);
        void Moderate(string messageId);
        void Report(string messageId);
    }
}
