using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Core.Repositories
{
    public interface IReportableRepository
    {
        void Report(string id);
        void Moderate(string id);
    }
}
