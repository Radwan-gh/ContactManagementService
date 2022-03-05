using CMS.Core.Contacts.Dtos;
using CMS.SharedKernel;
using System.Threading.Tasks;

namespace CMS.Core.Interfaces
{
    public interface IContactService
    {
        Task<ServiceResult> CreateContact(ContactDTO contact);
        Task<ServiceResult> DeleteContact(int id);
        Task<ServiceResult<ContactDetailsDTO>> GetContactById(int id);
        Task<ServiceResult<PaginatedResponse<ContactDTO>>> Search(ContactSearchDto dto);
        Task<ServiceResult> UpdateContact(ContactDTO contact);
    }
}
