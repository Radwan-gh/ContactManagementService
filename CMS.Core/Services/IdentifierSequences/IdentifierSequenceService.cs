using CMS.Core.Entities;
using CMS.Core.Interfaces;
using MongoDB.Driver;

namespace CMS.Core.Services.IdentifierSequences
{
    public class IdentifierSequenceService : IIdentifierSequenceService
    {
        private readonly IMongodbContext _mongodbContext;
        public IdentifierSequenceService(IMongodbContext mongodbContext)
        {
            _mongodbContext = mongodbContext;
        }
        public int GetIdentifier(EntityType type)
        {
            var options = new FindOneAndUpdateOptions<IdentifierSequence, IdentifierSequence>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            // Prepare Ticket ID
            var idFilter = Builders<IdentifierSequence>.Filter.And(
                 Builders<IdentifierSequence>.Filter.Eq(s => s.EntityType, type));

            var idUpdate = Builders<IdentifierSequence>.Update.Inc("Sequence", 1);
            var idSequence = _mongodbContext.IdentifierSequences.FindOneAndUpdate(idFilter, idUpdate, options).Sequence;

            return idSequence;
        }
    }
}
