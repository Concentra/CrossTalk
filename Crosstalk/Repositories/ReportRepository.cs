using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crosstalk.Common.Repositories;
using Crosstalk.Common.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Crosstalk.Core.Repositories
{
    public class ReportRepository : BaseMongoRepository<Report>, IReportRepository
    {
        public ReportRepository(MongoDatabase db) : base(db) { }

        protected override string Collection
        {
            get
            {
                return "report";
            }
        }

        public Report GetById(string id)
        {
            return this.GetCollection().AsQueryable().FirstOrDefault<Report>(r => r.Id == id);
        }

        public IEnumerable<Report> All()
        {
            return this.GetCollection().FindAllAs<Report>().AsEnumerable();
        }

        public IEnumerable<Report> GetReportsForParent(ReportType type, string parentId)
        {
            return this.GetCollection().AsQueryable().Where(r => r.Parent.Id == parentId && r.Type == type);
        }

        public void Save(Report report)
        {
            report.Id = ObjectId.GenerateNewId().ToString();
            this.GetCollection().Save(report);
        }

        /// <summary>
        /// This is equivalent to ignoring the report.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            this.GetCollection().Remove(Query.EQ("_id", id));
        }
    }
}