using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Repositories
{
    public interface IPartialRepository<T>
    {
        T GetById(string id);
    }
}
