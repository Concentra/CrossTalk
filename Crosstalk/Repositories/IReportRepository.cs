using Crosstalk.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crosstalk.Core.Repositories
{
    public interface IReportRepository
    {
        Report GetById(string id);
        IEnumerable<Report> All();
        IEnumerable<Report> GetReportsForParent(ReportType type, string parentId);
        void Save(Report report);
        void Delete(string id);
    }
}