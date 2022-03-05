using CMS.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CMS.Infrastructure.Data
{
    public class MongodbContext : IMongodbContext
    {
        private MongoClient _client { get; set; }
        public IMongoDatabase _database { get; set; }

        public MongodbContext(IConfiguration configuration)
        {
            var connectionString = configuration["Mongodb:ConnectionString"];
            var databaseName = configuration["Mongodb:DatabaseName"];
            _client = new MongoClient(connectionString);

            if (_client != null)
                _database = _client.GetDatabase(databaseName);

            init();
        }

        public IMongoCollection<Company> Companies { get { return _database.GetCollection<Company>(nameof(Company)); } }
        public IMongoCollection<Contact> Contacts { get { return _database.GetCollection<Contact>(nameof(Contact)); } }

        public void init()
        {
            BsonClassMap.RegisterClassMap<Company>();
            BsonClassMap.RegisterClassMap<Contact>();
        }
    }
}
