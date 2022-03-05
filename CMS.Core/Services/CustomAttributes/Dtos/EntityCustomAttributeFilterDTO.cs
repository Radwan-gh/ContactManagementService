using CMS.Core.Enums;

namespace CMS.Core.Services.CustomAttributes.Dtos
{
    public class EntityCustomAttributeViewDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AttributeType Type { get; set; }
    }
}
