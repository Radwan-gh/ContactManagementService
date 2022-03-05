using CMS.Core.Entities;
using CMS.Core.Enums;

namespace CMS.Core.Services.CustomAttributes.Dtos
{
    public class EntityCustomAttributeDTO
    {
        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public AttributeType Type { get; set; }
    }
}
