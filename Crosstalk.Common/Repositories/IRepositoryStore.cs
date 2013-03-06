using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Repositories
{
    public interface IRepositoryStore
    {
        IPartialResolver<TModel> Get<TModel>() where TModel : class;
        IRepositoryStore Add<TModel>(Func<IPartialResolver<TModel>> resolver)
            where TModel : class, new();
    }
}
