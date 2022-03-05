using CMS.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Core.Contacts.Dtos
{
    public class ContactCreateDTO
    {
        [Required]
        public string Name { get; set; }
        public ICollection<CustomAttributeRecord> CustomAttributes { get; set; }
        public ICollection<int> Companies { get; set; }
    }
}
