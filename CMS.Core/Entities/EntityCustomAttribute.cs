using CMS.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CMS.Core.Entities
{
    public class EntityCustomAttribute
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { set; get; }
        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public AttributeType Type { get; set; }
    }

}
