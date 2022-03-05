using CMS.Core.Entities;
using CMS.Core.Services.CustomAttributes.Dtos;
using CMS.SharedKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Core.Services.CustomAttributes
{
    public interface ICustomAttributeService
    {
        Task<ServiceResult> Create(EntityCustomAttributeDTO entityCustomAttributeDTO);
        Task<ServiceResult> Delete(string id);
        Task<ServiceResult<List<EntityCustomAttributeFilterDTO>>> GetAllForSearch();
        Task<ServiceResult<List<EntityCustomAttributeViewDTO>>> GetByEntityType(EntityType entityName);
        Task<ServiceResult> Update(EntityCustomAttributeDTO dto, string Id);
    }
}
