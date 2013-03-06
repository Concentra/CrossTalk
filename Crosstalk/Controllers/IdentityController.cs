using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Channels;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;
using Neo4jClient;
using Newtonsoft.Json.Linq;

namespace Crosstalk.Core.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IEdgeRepository _edgeRepository;
        private readonly IGraphClient _graphClient;

        public IdentityController(IIdentityRepository identityRepository, IGraphClient graphClient, IEdgeRepository edgeRepository)
        {
            this._identityRepository = identityRepository;
            this._edgeRepository = edgeRepository;
            this._graphClient = graphClient;
        }

        public dynamic GetById(string id)
        {
            var identity = this._identityRepository.GetById(id);
            if (HttpUtility.ParseQueryString(this.Request.RequestUri.Query).AllKeys.Where(k => k != "Id").Any())
            {
                return this.Search();
            }
            return identity;
        }

        [HttpGet]
        public IEnumerable<Identity> Search()
        {
            return this._identityRepository.Search(HttpUtility.ParseQueryString(this.Request.RequestUri.Query));
        }
        
        [HttpGet]
        public IEnumerable<Identity> Search(string field, string value)
        {
            return this._identityRepository.Search(field, value);
        }

        public Identity Post(JObject obj)
        {
            var model = obj.ToObject<Identity>();

            if (IdentityType.Public == model.Type)
            {
                throw new ArgumentException("The Public space can only be created internally.");
            }

            /**
             * Bring additional fields into Data
             */
            //var props = obj.Properties().Where(p => typeof (Identity).GetProperty(p.Name) == null && p.Name != "Parent");
            //foreach (var prop in props)
            //{
            //    model.Data[prop.Name] = prop.Value<String>();
            //}

            model.OId = ObjectId.GenerateNewId();

            /**
             * Create a graph representation of the Identity
             */
            model.GraphId = this._graphClient.Create(model.ToGraphIdentity()).Id;

            var actions = new List<Action>
                {
                    /**
                     * Create a loopback on the Group or Person
                     */
                    () => this._edgeRepository.Save(new Edge
                        {
                            From = model,
                            To = model,
                            cType = ChannelType.Public
                        })
                };

            /**
             * Automatically connect a Person to the public space
             */
            if (Identity.Person == model.Type)
            {
                actions.Add(() => this._edgeRepository.Save(new Edge
                    {
                        From = model,
                        To = this._identityRepository.GetPublicSpace(),
                        cType = ChannelType.Public
                    }).Save(new Edge
                    {
                        From = this._identityRepository.GetPublicSpace(),
                        To = model,
                        cType = ChannelType.Public
                    }));
            }

            /**
             * If the Group or Person has a parent, connect them to it.
             */
            if (null != model.Data && model.Data.ContainsKey("Parent") && null != (model.Data["Parent"] as string))
            {
                var parent = this._identityRepository.GetById(model.Data["Parent"] as string);
                if (null != parent)
                {
                    switch (model.Type)
                    {
                        case Identity.Group:
                            
                            if (Identity.Person == parent.Type)
                            {
                                throw new ArgumentException(
                                    "Groups can only be a member of the public space and other Groups", "obj");
                            }
                            
                            /**
                             * This creates a directed public connection from the parent to the group
                             */

                            actions.Add(() => this._edgeRepository.Save(new Edge
                                {
                                    From = parent,
                                    To = model,
                                    cType = ChannelType.Public
                                }));
                            break;
                        case Identity.Person:
                            
                            if (Identity.Group != parent.Type)
                            {
                                throw new ArgumentException("People can only be a member of a Group", "obj");
                            }

                            /**
                             * This creates a bi-directional public channel between a person and a group
                             * and a directed private connection from a person to a group
                             */
                            actions.Add(() => this._edgeRepository.Save(new Edge
                                {
                                    From = parent,
                                    To = model,
                                    cType = ChannelType.Public
                                }).Save(new Edge
                                {
                                    From = model,
                                    To = parent,
                                    cType = ChannelType.Public
                                }).Save(new Edge
                                {
                                    From = model,
                                    To = parent,
                                    cType = ChannelType.Private
                                }));
                            break;
                    }
                }
            }
            
            /**
             * Store the identity and connections
             */
            Parallel.Invoke(new Action[] {
                () => this._identityRepository.Save(model),
                () => {
                    /**
                     * Graph actions hitting the same nodes will create a deadlock in a multi-threaded setting.
                     * See: http://docs.neo4j.org/chunked/milestone/transactions-locking.html
                     */
                    foreach(var action in actions) {
                        action.Invoke();
                    }
                }
            });
            return model;
        }

        /// <summary>
        /// Update a field in the identity
        /// </summary>
        /// <param name="id">ID of the Identity to update</param>
        /// <param name="model">Fields to update</param>
        /// <returns></returns>
        public Identity Post(string id, [FromBody] Dictionary<string, object> model)
        {
            var identity = this._identityRepository.GetById(id);
            var type = identity.GetType();
            foreach (var kv in model)
            {
                var field = type.GetProperty(kv.Key);
                if (null != field)
                {
                    field.SetValue(identity, kv.Value);
                }
            }
            this._identityRepository.Save(identity);
            return identity;
        }

    }
}
