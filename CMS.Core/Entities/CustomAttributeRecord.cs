using CMS.Core.Enums;

namespace CMS.Core.Entities
{
    public record CustomAttributeRecord(string Name, string Value, AttributeType Type);
}
