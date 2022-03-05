using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CMS.Core.Entities
{
    public class IdentifierSequence
    {
        [BsonId]
        public BsonObjectId Id { get; set; }
        public int Sequence { get; set; }
        public EntityType EntityType { get; set; }
    }
}
