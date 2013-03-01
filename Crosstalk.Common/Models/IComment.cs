using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public interface IComment<TIdentity> : IReportable where TIdentity : class, IIdentity
    {
        Partial<TIdentity> Author { get; set; }
        String Body { get; set; }
        DateTime Created { get; set; }
        string Status { get; set; }
    }
}
