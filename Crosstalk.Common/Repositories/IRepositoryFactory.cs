using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Repositories
{
    public interface IRepositoryFactory
    {
        IPartialRepository<T> GetRepositoryFor<T>() where T : class;
        void AddRepositoryFor<TModel, TRepository>()
            where TModel : class, new()
            where TRepository : IPartialRepository<TModel>, new();
    }
}
