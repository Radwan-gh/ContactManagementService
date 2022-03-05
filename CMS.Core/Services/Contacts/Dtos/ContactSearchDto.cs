using CMS.Core.Entities;
using CMS.Core.Enums;
using System.Collections.Generic;

namespace CMS.Core.Contacts.Dtos
{
    public class ContactSearchDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
        public IEnumerable<SearchAttribute> SearchAttributes { get; set; }
    }
    public record SearchAttribute(
        EntityType EntityType,
        string AttributeName,
        string SearchTerm,
        AttributeType Type,
        FilterType FilterType);
}
