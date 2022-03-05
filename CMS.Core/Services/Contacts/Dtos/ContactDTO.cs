using CMS.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CMS.Core.Contacts.Dtos
{
    public class ContactDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public ICollection<CustomAttributeRecord> CustomAttributes { get; set; }
        public ICollection<int> Companies { get; set; }
    }
}
