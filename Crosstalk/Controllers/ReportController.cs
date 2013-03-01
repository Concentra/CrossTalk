using Crosstalk.Common.Models;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Crosstalk.Core.Controllers
{
    public class ReportController : ApiController
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportService _reportService;

        public ReportController(IReportRepository reportRepository, IReportService reportService)
        {
            this._reportRepository = reportRepository;
            this._reportService = reportService;
        }

        public void Post(Report report)
        {
            this._reportService.Report(report);
        }

        public dynamic Get()
        {
            var nvc = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
            if (nvc.AllKeys.Contains("id"))
            {
                return this._reportRepository.GetById(nvc["id"]);
            }
            return this._reportService.All();
        }

        public void Delete(string id)
        {
            // Ignore report.
            this._reportService.Ignore(id);
        }

    }
}
