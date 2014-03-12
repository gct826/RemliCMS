using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemliCMS.WebData.Entities;
using MongoDB.Driver;

namespace RemliCMS.WebData
{

    public class MongoConnectionHandler<T> where T : IMongoEntity
    {

        public MongoCollection<T> MongoCollection { get; private set; }

        
        public string MongoDbLocation { get; set; }
 
        public MongoConnectionHandler()
        {
            var dbUser = System.Configuration.ConfigurationManager.AppSettings["MongoDbUser"];
            var dbPwd = System.Configuration.ConfigurationManager.AppSettings["MongoDbPwd"];
            var dbLocation = System.Configuration.ConfigurationManager.AppSettings["MongoDbLocation"];

            if (dbUser != "")
            {
                dbLocation = "mongodb://" + dbUser + ":" + dbPwd + "@" + dbLocation;
            }
            else
            {
                dbLocation = "mongodb://" + dbLocation;
            }

            var mongoConfig = new MongoDbConfig
            {   
                DbLocation = dbLocation,
                DbName = System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]
            };

            string connectionString = mongoConfig.DbLocation;
            string databaseName = mongoConfig.DbName;

            // Get a thread-safe client object by using a connection string
            var mongoClient = new MongoClient(connectionString);

            // Get a reference to a server object from the Mongo client object
            var mongoServer = mongoClient.GetServer();

            // Get a reference to the databse object from the Mongo server object
            var db = mongoServer.GetDatabase(databaseName);

            // Get a refernce to the colleciton object from the Mongo database object
            MongoCollection = db.GetCollection<T>(typeof (T).Name.ToLower() + "s");
        }
    }

}
