using Autofac;
using Crosstalk.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crosstalk.Core
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IContainer _container;
        private readonly Dictionary<Type, Type> _registry = new Dictionary<Type, Type>();

        public RepositoryFactory(IContainer container)
        {
            this._container = container;
        }

        public IPartialRepository<T> GetRepositoryFor<T>() where T : class
        {
            if (!this._registry.ContainsKey(typeof(T)))
            {
                throw new ArgumentException(typeof(T).FullName + " is not in Type Registry");
            }
            return (IPartialRepository<T>) this._container.Resolve(this._registry[typeof(T)]);
        }

        public void AddRepositoryFor<TModel, TRepository>()
            where TModel : class, new()
            where TRepository : IPartialRepository<TModel>, new()
        {
            this._registry.Add(typeof(TModel), typeof(TRepository));
        }
    }
}