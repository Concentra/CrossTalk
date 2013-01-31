using System.Collections.Generic;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;
using Neo4jClient;

namespace Crosstalk.Core.Services
{
    public class BootstrapService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IEdgeRepository _edgeRepository;

        private readonly GraphClient _client;

        public BootstrapService(IIdentityRepository identityRepository, IEdgeRepository edgeRepository, GraphClient client)
        {
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;

            this._client = client;
        }

        public Identity CreateIdentity(string name)
        {
            var me = new Identity()
                {
                    Name = name,
                    OId = ObjectId.GenerateNewId()
                };
            me.GraphId = this._client.Create(me.ToGraphIdentity()).Id;
            this._identityRepository.Save(me);
            return me;
        }

        public Identity CreatePublic()
        {
            return this.CreateIdentity(Identity.Public, "", Identity.Public, null, null);
        }

        public Identity CreateGroup(string name, IEnumerable<Identity> outbound, IEnumerable<Identity> inbound)
        {
            return this.CreateIdentity(name, "", Identity.Group, outbound, inbound);
        }

        public Identity CreatePerson(string name, string avatar, IEnumerable<Identity> outbound,
                                     IEnumerable<Identity> inbound)
        {
            return this.CreateIdentity(name, avatar, Identity.Person, outbound, inbound);
        }

        public Identity CreateIdentity(string name,
                                       string avatar,
                                       string type,
                                       IEnumerable<Identity> outbound,
                                       IEnumerable<Identity> inbound)
        {
            var me = new Identity
                {
                    AvatarUrl = avatar,
                    Name = name,
                    Type = type,
                    OId = ObjectId.GenerateNewId()
                };
            me.GraphId = this._client.Create(me.ToGraphIdentity()).Id;
            this._identityRepository.Save(me);

            if (null != outbound)
            {
                foreach (var assoc in outbound)
                {
                    this._edgeRepository.Save(new Edge
                        {
                            To = assoc,
                            From = me
                        });
                }
            }

            if (null != inbound)
            {
                foreach (var assoc in inbound)
                {
                    this._edgeRepository.Save(new Edge
                        {
                            From = assoc,
                            To = me
                        });
                }
            }

            return me;
        }

        public Identity CreateIdentityWithAssociation(string name, Identity association)
        {
            var me = new Identity()
                {
                    OId = ObjectId.GenerateNewId(),
                    Name = name
                };
            me.GraphId = this._client.Create(me.ToGraphIdentity()).Id;
            this._identityRepository.Save(me);
            this._edgeRepository
                .Save(new Edge()
                {
                    From = association,
                    To = me
                }).Save(new Edge()
                {
                    From = me,
                    To = association
                });
            return me;
        }
    }
}