using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.Constants;
using Project_BDS.Application.ImplementService;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using System.Reflection.Metadata;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Request_Register request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.ConfirmCode))
            {
                return BadRequest(new { message = "Mã xác nhận là bắt buộc" });
            }

            var result = await _authService.ConfirmRegisterAccount(request.ConfirmCode);
            if (result.Contains("thành công"))
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email không hợp lệ." });
            }

            var result = await _authService.ResendConfirmationCode(email);

            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpPost]
        public async Task<IActionResult> Login(Request_Login request)
        {
            return Ok(await _authService.Login(request));
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] Request_ChangePassword request)
        {
            // Lấy User ID từ claim "Id" trong token
            var userIdClaim = HttpContext.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Không tìm thấy User ID trong token.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID không hợp lệ trong token.");
            }

            // Gọi service để thay đổi mật khẩu
            var result = await _authService.ChangePassword(userId, request);

            // Trả về kết quả tương ứng
            return StatusCode(result.Status, result);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Request_ForgotPassword request)
        {
            var result = await _authService.ForgotPassword(request);

            // Nếu lỗi 400 - BadRequest
            if (result.Status == StatusCodes.Status400BadRequest)
            {
                return BadRequest(new { message = result.Message });
            }

            // Nếu lỗi 500 - Internal Server Error
            if (result.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.Message });
            }

            // Trả về OK nếu thành công
            return Ok(new { message = result.Message });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Request_ResetPassword request)
        {
            var response = await _authService.ResetPassword(request);

            if (response.Status == StatusCodes.Status200OK)
            {
                return Ok(new
                {
                    status = response.Status,
                    message = response.Message
                });
            }

            return StatusCode(response.Status, new
            {
                status = response.Status,
                message = response.Message
            });
        }

    }
}
