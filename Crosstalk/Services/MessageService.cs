using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;
using Crosstalk.Common.Models;

namespace Crosstalk.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IEdgeService _edgeService;

        public MessageService(IMessageRepository messageRepository, IEdgeService edgeService)
        {
            this._messageRepository = messageRepository;
            this._edgeService = edgeService;
        }

        public IList<Message> GetListForEdge(Edge edge)
        {
            return this.GetListForEdge(edge, null);
        }

        public IList<Message> GetListForEdge(Edge edge, int? count)
        {
            var list = this._messageRepository.GetListForEdge(edge, count);

            Parallel.ForEach(list,
                             m =>
                             {
                                 if (null != m.OriginalMessageId && !ObjectId.Empty.ToString().Equals(m.OriginalMessageId))
                                 {
                                     m.OriginalMessage = this._messageRepository.GetById(m.OriginalMessageId);
                                     if (null == m.OriginalMessage)
                                     {
                                         m.Status = ReportableStatus.Missing;
                                     }
                                     else
                                     {
                                         if (null != m.OriginalMessage.Edge)
                                         {
                                             m.OriginalMessage.Edge = this._edgeService.GetById(m.OriginalMessage.Edge.Id.ToString());
                                         }
                                         m.OriginalMessage.NumberOfShares =
                                             (int)this._messageRepository.CountShares(m.OriginalMessageId);
                                     }
                                 }
                                 m.Edge = edge;
                             });

            return list;
        }
    }
}