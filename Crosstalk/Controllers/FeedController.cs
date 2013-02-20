using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Services;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Collections;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Controllers
{
    public class FeedController : ApiController
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IEdgeRepository _edgeRepository;
        private readonly IMessageService _messageService;

        public FeedController(IMessageService messageService,
            IIdentityRepository identityRepository,
            IEdgeRepository edgeRepository)
        {
            this._identityRepository = identityRepository;
            this._messageService = messageService;
            this._edgeRepository = edgeRepository;
        }

        public IEnumerable<Message> GetById(string id, JArray exclusions)
        {
            var me = this._identityRepository.GetById(id);
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, ChannelType.Public, 3))
                .Where(e => null == exclusions || !exclusions.Contains(e.Id));
            var messages = new OrderedList<Message>((l, n) =>
                l.Created == n.Created ? 0 : l.Created > n.Created ? 1 : -1);
            Parallel.ForEach(edges, edge =>
            {
                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
                lock (messages)
                {
                    messages.AddRange(this._messageService.GetListForEdge(edge));
                }
            });
            return messages;
        }

    }
}
