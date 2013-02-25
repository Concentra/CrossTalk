using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Core.Models.Messages;

namespace Crosstalk.Core.Services
{
    public interface IMessageService
    {
        IList<Message> GetListForEdge(Edge edge);
        IList<Message> GetListForEdge(Edge edge, int? count);
    }
}