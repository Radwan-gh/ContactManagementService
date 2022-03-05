using CMS.Core.Contacts.Dtos;
using CMS.Core.Interfaces;
using CMS.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class ContactController : ApiControllerBase
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactDTO dto)
        {
            var result = await _contactService.CreateContact(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] ContactSearchDto dto)
        {
            var result = await _contactService.Search(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ContactDTO dto, int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            dto.Id = id;

            var result = await _contactService.UpdateContact(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            var result = await _contactService.DeleteContact(id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            var result = await _contactService.GetContactById(id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

    }
}
