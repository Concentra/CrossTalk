using System.Web.Mvc;
using Crosstalk.Common.Repositories;
using System;
using System.Linq;
using Newtonsoft.Json;
using Crosstalk.Common.Convertors;

namespace Crosstalk.Common.Models
{

    public abstract class Partial
    {
        public string Id { get; set; }
    }

    [JsonConverter(typeof(PartialConvertorProxy))]
    public class Partial<T> : Partial
        where T : class, ISupportsPartial
    {

        private readonly IPartialResolver<T> _repository;
        private T _instance;

        public Partial()
        {
            /**
             * This is ugly as hell
             */
            var repoResolver = RepositoryStore.GetInstance();
            if (null != repoResolver)
            {
                /**
                 * TODO: Define a specific exception
                 */
                try
                {
                    this._repository = repoResolver.Get<T>();
                }
                catch (Exception e)
                {
                    this._repository = null;
                }
            }
        }

        public Partial(Partial part)
        {
            this.Id = part.Id;
        }

        public Partial(T value)
        {
            this._instance = value;
            this.Id = value.Id;
        }

        public T Value
        {
            get
            {
                if (null == this._instance)
                {
                    if (this.CanLoad)
                    {
                        this._instance = this._repository.GetById(this.Id);
                    }
                    else if (typeof(T).IsClass)
                    {
                        var obj = (T)Activator.CreateInstance(typeof(T));
                        var prop = typeof(T).GetProperty("Id");
                        if (null != prop)
                        {
                            prop.SetValue(obj, this.Id);
                        }
                        return obj;
                    }
                }
                return this._instance;
            }
            set
            {
                this._instance = value;
            }
        }

        private bool CanLoad
        {
            get
            {
                return null != this._repository;
            }
        }

        public bool IsPartial
        {
            get
            {
                return null == this._instance;
            }
        }

        public static explicit operator T(Partial<T> partial)
        {
            return partial.Value;
        }

        public static implicit operator Partial<T>(T value)
        {
            if (null == value)
            {
                return null;
            }
            var obj = new Partial<T>(value);
            var prop = typeof(T).GetProperty("Id");
            if (null != prop)
            {
                obj.Id = prop.GetValue(value) as string;
            }
            return obj;
        }

        public bool ShouldSerializeValue()
        {
            return false;
        }

    }
}
