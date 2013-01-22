using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Models;
using Crosstalk.Repositories;
using MongoDB.Bson;
using Neo4jClient;

namespace Crosstalk.Services
{
    public class IdentityService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IEdgeRepository _edgeRepository;

        private readonly GraphClient _client;

        public IdentityService(IIdentityRepository identityRepository, IEdgeRepository edgeRepository, GraphClient client)
        {
            this._edgeRepository = edgeRepository;
            this._identityRepository = identityRepository;

            this._client = client;
        }

        public Identity CreateIdentity(string name)
        {
            var me = new Identity()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = name
                };
            me.GraphId = this._client.Create<Identity>(me).Id;
            this._identityRepository.Save(me);
            return me;
        }

        public Identity CreateIdentityWithAssociation(string name, Identity association)
        {
            var me = new Identity()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = name
                };
            me.GraphId = this._client.Create<Identity>(me).Id;
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