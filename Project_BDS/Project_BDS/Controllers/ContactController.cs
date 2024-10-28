using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Domain.Entities;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("send")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendContact([FromBody] ContactRequest contactRequest)
        {
            var response = await _contactService.SendContactAsync(contactRequest);
            return StatusCode(response.Status, response);
        }
        [HttpGet("GetContactsForRoleId3")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetContactsForRoleId3()
        {
            var result = await _contactService.GetContactsForRoleId3Async();

            // Trả về status code và thông điệp tùy thuộc vào kết quả từ service
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(new { data = result.Data, message = result.Message });
            }
            else
            {
                return StatusCode(result.Status, new { message = result.Message });
            }
        }

    }
}
