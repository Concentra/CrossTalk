using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public interface IComment
    {
        Partial<IIdentity> Author { get; set; }
        String Body { get; set; }
        DateTime Created { get; set; }
        ReportableStatus Status { get; set; }
    }
}
