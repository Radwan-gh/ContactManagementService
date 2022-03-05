using CMS.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Core.Contacts.Dtos
{
    public class ContactDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CustomAttributeRecord> CustomAttributes { get; set; }
        public ICollection<Company> Companies { get; set; }

        public static ContactDetailsDTO FromEntity(Contact contact)
        {
            var dto = new ContactDetailsDTO();
            dto.Id = contact.Id;
            dto.Name = contact.CustomAttributes.FirstOrDefault(c => c.Name == nameof(dto.Name))?.Value;
            dto.CustomAttributes = contact.CustomAttributes.Where(c => c.Name != nameof(dto.Name)).ToList();
            dto.Companies = contact.Companies;

            return dto;
        }
    }
}
