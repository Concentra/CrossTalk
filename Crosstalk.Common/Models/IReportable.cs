using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Models
{
    public interface IReportable : ISupportsPartial
    {
        IEnumerable<Report> Reports { get; set; }
        ReportableStatus Status { get; set; }
        string Body { get; set; }
    }
}
