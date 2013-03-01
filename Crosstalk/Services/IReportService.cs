using Crosstalk.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crosstalk.Core.Services
{
    public interface IReportService
    {
        void Report(Report report);
        void Ignore(string id);
        void Remove(string id);
    }
}