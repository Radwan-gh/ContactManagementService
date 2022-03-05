using CMS.Core.Contacts.Dtos;
using CMS.Core.Interfaces;
using CMS.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CompanyController : ApiControllerBase
    {

        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyDTO dto)
        {
            var result = await _companyService.Create(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CompanyDTO dto, int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            dto.Id = id;
            var result = await _companyService.Update(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            var result = await _companyService.DeleteCompany(id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            if (id == 0)
                return BadRequest("id is required!");

            var result = await _companyService.GetById(id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get List of Companies for fill dropdown list in create new contact page 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(int pageIndex, int pageSize)
        {
            var result = await _companyService.GetAll(pageIndex, pageSize);
            return Ok(result);
        }


    }
}
