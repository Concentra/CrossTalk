using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;

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
                                 if (ObjectId.Empty != m.OriginalMessageId)
                                 {
                                     m.OriginalMessage = this._messageRepository.Get(m.OriginalMessageId.ToString());
                                     if (null != m.OriginalMessage.Edge)
                                     {
                                         m.OriginalMessage.Edge = this._edgeService.GetById(m.OriginalMessage.Edge.Id);
                                     }
                                     m.OriginalMessage.NumberOfShares =
                                         (int) this._messageRepository.CountShares(m.OriginalMessageId.ToString());
                                 }
                                 m.Edge = edge;
                             });

            return list;
        }
    }
}