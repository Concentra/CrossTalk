using Autofac;
using Crosstalk.Common;
using Crosstalk.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crosstalk.Common
{
    public class RepositoryStore : IRepositoryStore
    {
        private readonly Dictionary<Type, Func<object>> _registry = new Dictionary<Type, Func<object>>();

        public IPartialResolver<TModel> Get<TModel>() where TModel : class
        {
            if (!_registry.ContainsKey(typeof(TModel)))
            {
                throw new ArgumentException(typeof(TModel).FullName + " is not in Type Registry");
            }
            return (IPartialResolver<TModel>)this._registry[typeof(TModel)].Invoke();
        }

        public IRepositoryStore Add<TModel>(Func<IPartialResolver<TModel>> resolver)
            where TModel : class, new()
        {
            _registry.Add(typeof(TModel), resolver);
            return this;
        }

        private static RepositoryStore instance;

        public static RepositoryStore GetInstance()
        {
            if (null == instance)
            {
                instance = new RepositoryStore();
            }
            return instance;
        }
    }
}