using CMS.Core.Common;
using CMS.Core.Companys.Dtos;
using CMS.Core.Contacts.Dtos;
using CMS.SharedKernel;
using System.Threading.Tasks;

namespace CMS.Core.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult> Create(CompanyDTO companyDTO);
        Task<ServiceResult> DeleteCompany(int id);
        Task<PaginatedResponse<SelectItem>> GetAll(int page, int pageSize);
        Task<ServiceResult<CompanyDetailsDTO>> GetById(int id);
        Task<ServiceResult> Update(CompanyDTO companyDto);
    }
}
