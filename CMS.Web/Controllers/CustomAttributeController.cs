using CMS.Core.Entities;
using CMS.Core.Services.CustomAttributes;
using CMS.Core.Services.CustomAttributes.Dtos;
using CMS.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CustomAttributeController : ApiControllerBase
    {
        private readonly ICustomAttributeService _customAttributeService;
        public CustomAttributeController(ICustomAttributeService customAttributeService)
        {
            _customAttributeService = customAttributeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EntityCustomAttributeDTO dto)
        {
            var result = await _customAttributeService.Create(dto);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EntityCustomAttributeDTO dto,
            string id)
        {
            if (string.IsNullOrWhiteSpace(id) || ObjectId.TryParse(id, out ObjectId _))
                return BadRequest("id is not valid!");

            var result = await _customAttributeService.Update(dto, id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id) || ObjectId.TryParse(id, out ObjectId _))
                return BadRequest("id is required!");

            var result = await _customAttributeService.Delete(id);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EntityType type)
        {
            var result = await _customAttributeService.GetByEntityType(type);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all entities custom attributes to be used in search 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllForSearch()
        {
            var result = await _customAttributeService.GetAllForSearch();

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
}
