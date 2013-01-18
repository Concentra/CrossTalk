using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace Crosstalk.App_Start
{
    public class MongoConfig
    {
        private static MongoDatabase _database;

        public static MongoDatabase GetDb()
        {
            if (null == _database)
            {
                var client = new MongoDB.Driver.MongoClient(ConfigurationManager.AppSettings["Mongo:Connection"]);
                var server = client.GetServer();
                _database = server.GetDatabase(ConfigurationManager.AppSettings["Mongo:Db"]);
            }
            return _database;
        }
    }
}