using System.Collections.Generic;
using Crosstalk.Core.Models;

namespace Crosstalk.Core.Repositories
{
    public interface IMessageRepository
    {
        IList<Message> GetList();
        IList<Message> GetListForEdge(Edge edge);
        Message Get(string messageId);
        bool Save(Message message);
        // void Moderate(int messageId);
        // void Report(int messageId);
    }
}
