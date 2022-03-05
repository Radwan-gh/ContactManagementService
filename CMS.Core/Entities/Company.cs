using System.Collections.Generic;

namespace CMS.Core.Entities
{
    public class Company : BaseEntity
    {
        public IList<CustomAttributeRecord> CustomAttributes { get; set; } =
              new List<CustomAttributeRecord>();
    }
}
