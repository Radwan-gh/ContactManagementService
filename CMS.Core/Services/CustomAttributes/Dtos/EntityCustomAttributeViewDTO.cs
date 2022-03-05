using CMS.Core.Entities;
using CMS.Core.Enums;

namespace CMS.Core.Services.CustomAttributes.Dtos
{
    public record EntityCustomAttributeFilterDTO(string Id, string Name, EntityType EntityType, AttributeType Type);
}
