using System.Web.Mvc;
using Crosstalk.Common.Repositories;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using Newtonsoft.Json;
using Crosstalk.Common.Convertors;

namespace Crosstalk.Common.Models
{
    public abstract class Partial
    {
        public string Id { get; set; }
        public object Value { get; set; }

        protected JsonConverter converter;
        public JsonConverter Convertor
        {
            get
            {
                return this.converter;
            }
        }
    }

    [JsonConverter(typeof(PartialConvertorProxy))]
    public class Partial<T> : Partial where T : class
    {

        private readonly IRepositoryFactory _factory;
        private T _instance;

        public Partial()
        {
            /**
             * This is ugly as hell
             */
            this._factory = DependencyResolver.Current.GetService<IRepositoryFactory>();
            this.converter = new PartialConvertor<T>();
        }

        [BsonIgnore]
        public new T Value
        {
            get
            {
                if (null == this._instance)
                {
                    this._instance = this._factory.GetRepositoryFor<T>().GetById(this.Id);
                }
                return this._instance;
            }
        }
    }
}
