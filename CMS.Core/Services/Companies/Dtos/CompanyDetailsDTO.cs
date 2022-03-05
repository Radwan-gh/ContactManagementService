using CMS.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Core.Companys.Dtos
{
    public class CompanyDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CustomAttributeRecord> CustomAttributes { get; set; }
        public static CompanyDetailsDTO FromEntity(Company company)
        {
            var dto = new CompanyDetailsDTO();
            dto.Id = company.Id;
            dto.Name = company.CustomAttributes.FirstOrDefault(c => c.Name == nameof(dto.Name))?.Value;
            dto.CustomAttributes = company.CustomAttributes.Where(c => c.Name != nameof(dto.Name)).ToList();
            return dto;
        }
    }
}
