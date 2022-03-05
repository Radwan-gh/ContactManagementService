using System.Collections.Generic;

namespace CMS.Core.Entities
{
    public class Contact : BaseEntity
    {
        public IList<CustomAttributeRecord> CustomAttributes { get; set; } =
                new List<CustomAttributeRecord>();
        public IList<Company> Companies { get; set; } = new List<Company>();
    }
}