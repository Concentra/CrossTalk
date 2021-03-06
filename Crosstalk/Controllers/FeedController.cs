﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages;
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
            IEnumerable<string> excludedIdentities = null;
            try
            {
                excludedIdentities = JArray.Parse(exclusions).ToObject<IEnumerable<string>>();
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentException("Excluded Identities were not provided in the format expected", "exclusions");
            }
            
            var me = this._identityRepository.GetById(id);

            /**
             * Pass through nodes to exclude.
             */
            var edges = excludedIdentities.Contains("all")
                ? this._edgeRepository.GetToNode(me, ChannelType.Public, 0).ToList()
                  .Union(this._edgeRepository.GetFromNode(me, ChannelType.Public).ToList())
                : new List<Edge>(this._edgeRepository.GetToNode(
                            me,
                            ChannelType.Public,
                            null,
                            excludedIdentities.Contains("public"),
                            excludedIdentities.Where(i => "public" != i)));

            var messages = new OrderedList<Message>((l, n) =>
                l.Created == n.Created ? 0 : l.Created > n.Created ? 1 : -1);
            
            Parallel.ForEach(edges, edge =>
            {
                edge.To = edge.To.Id == me.Id ? me : this._identityRepository.GetById(edge.To.Id);
                edge.From = edge.From.Id == me.Id ? me : this._identityRepository.GetById(edge.From.Id);
                lock (messages)
                {
                    var temp = this._messageService.GetListForEdge(edge);
                    foreach (var message in temp)
                    {
                        var messageId = message.OriginalMessageId ?? message.Id;
                        if (!messages.Any(m => (m.OriginalMessageId ?? m.Id) == messageId))
                        {
                            messages.Add(message);
                        }
                        else
                        {
                            var storedMessage = messages.Where(m => (m.OriginalMessageId ?? m.Id) == messageId).Single();
                            if (message.Created > storedMessage.Created)
                            {
                                messages.Remove(storedMessage);
                                messages.Add(message);
                            }
                        }
                    }
                }
            });
            return messages;
        }
    }
}
