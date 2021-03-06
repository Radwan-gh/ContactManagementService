using CMS.Core.Entities;
using MongoDB.Driver;

namespace CMS.Core.Interfaces
{
    public interface IMongodbContext
    {
        IMongoDatabase _database { get; set; }

        IMongoCollection<Company> Companies { get; }
        IMongoCollection<Contact> Contacts { get; }
        IMongoCollection<IdentifierSequence> IdentifierSequences { get; }
        IMongoCollection<EntityCustomAttribute> EntityCustomAttributes { get; }
    }
}