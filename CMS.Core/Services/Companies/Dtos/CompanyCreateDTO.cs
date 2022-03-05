using CMS.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Core.Contacts.Dtos
{
    public class CompanyCreateDTO
    {
        [Required]
        public string Name { get; set; }
        public ICollection<CustomAttributeRecord> CustomAttributes { get; set; }
    }
}
