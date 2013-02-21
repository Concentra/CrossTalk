using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Services;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

        public IEnumerable<Message> GetById(string id, string exclusions)
        {
            IEnumerable<dynamic> excludedIdentities = null;
            try
            {
                excludedIdentities = JArray.Parse(exclusions).ToObject<IEnumerable<dynamic>>();
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentException("Excluded Identities were not provided in the format expected", "exclusions");
            }
            
            var me = this._identityRepository.GetById(id);
            
            /**
             * Exclude as many edges here as possible.
             */
            var edges = new List<Edge>(this._edgeRepository.GetToNode(me, ChannelType.Public, 3)
                .Where(e => null == excludedIdentities
                    || !excludedIdentities.Contains(e.Id)));

            IEnumerable<Edge> friends = null;

            if (excludedIdentities.Contains("public"))
            {
                friends = this._edgeRepository.GetFromNode(me, ChannelType.Public);
            }

            var messages = new OrderedList<Message>((l, n) =>
                l.Created == n.Created ? 0 : l.Created > n.Created ? 1 : -1);
            
            Parallel.ForEach(edges, edge =>
            {
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);

                /**
                 * Exclude messages from people that aren't to a group or me.
                 */
                if (excludedIdentities.Contains("network")
                    && IdentityType.Group != edge.To.Type
                    && edge.To.Id != me.Id
                    && edge.From.Id != me.Id)
                {
                    return;
                }

                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);

                /**
                 * Exclude messages to the public space
                 */
                if (excludedIdentities.Contains("public") && IdentityType.Public == edge.To.Type)
                {
                    if (null == friends || !friends.Any(e => edge.From.Id == e.To.Id))
                    {
                        return;
                    }
                }


                lock (messages)
                {
                    messages.AddRange(this._messageService.GetListForEdge(edge));
                }
            });
            return messages;
        }

    }
}
