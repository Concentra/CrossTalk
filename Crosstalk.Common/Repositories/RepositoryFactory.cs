//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Crosstalk.Common.Repositories;
//using Autofac;

//namespace Crosstalk.Common.Repositories
//{
//    public class RepositoryFactory : IRepositoryFactory
//    {
//        private readonly IComponentContext _context;
//        private readonly Dictionary<Type, Type> _registry = new Dictionary<Type, Type>();

//        public RepositoryFactory(IComponentContext context)
//        {
//            this._context = context;
//        }

//        public IPartialResolver<T> GetRepositoryFor<T>() where T : class
//        {
//            if (!this._registry.ContainsKey(typeof(T)))
//            {
//                throw new ArgumentException(typeof(T).FullName + " is not in Type Registry");
//            }
//            return this._context.Resolve(this._registry[typeof(T)]) as IPartialResolver<T>;
//        }

//        public void AddRepositoryFor<TModel, TRepository>()
//            where TModel : class, new()
//            where TRepository : IPartialResolver<TModel>, new()
//        {
//            this.AddRepositoryFor(typeof(TModel), typeof(TRepository));
//        }

//        public void AddRepositoryFor(Type model, Type repo)
//        {
//            if (!model.IsClass)
//            {
//                throw new ArgumentException("Class expected", "model");
//            }
//            if (!typeof(IPartialResolver<>).IsAssignableFrom(repo))
//            {
//                throw new ArgumentException("Repository should inherit IPartialResolver", "repo");
//            }
//            this._registry.Add(model, repo);
//        }
//    }
//}