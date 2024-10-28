using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using System.Threading.Tasks;
using System.Linq;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Project_BDS.Application.ImplementService;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
        {
            // Get the admin user's ID from claims
            var adminIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (int.TryParse(adminIdClaim, out int adminId))
            {
                var result = await _userService.UpdateUserRole(adminId, request);
                return StatusCode(result.Status, result);
            }

            return Unauthorized(new ResponseObject<DataResponseUser>
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = "Không xác thực người dùng!",
                Data = null
            });
        }

        [HttpPut("UpdateUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateRoleUser([FromBody] UpdateUserRoleRequest request) // Chỉnh sửa từ FromRoute thành FromBody
        {
            return Ok(await _userService.UpdateRoleUser(request));
        }

        [HttpGet("CheckClaims")]
        public IActionResult CheckClaims()
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            return Ok(claims.Select(c => new { c.Type, c.Value }));
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsers();
            return StatusCode(response.Status, response);
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetAllUsers([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        //{
        //    var result = await _userService.GetAllUsers(pageSize, pageNumber);
        //    return StatusCode(result.Status, result);
        //}

        #region Lấy thông tin người dùng theo Email
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmail(email);
            return StatusCode(result.Status, result);
        }
        #endregion

        #region Lấy thông tin người dùng theo FullName
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersByFullName([FromQuery] string fullName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUsersByFullName(fullName, pageSize, pageNumber);
            return StatusCode(result.Status, result);
        }
        #endregion

        #region Lấy thông tin người dùng theo PhoneNumber
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNumber)
        {
            var result = await _userService.GetUserByPhoneNumber(phoneNumber);
            return StatusCode(result.Status, result);
        }
        #endregion
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            return StatusCode(result.Status, result);
        }
        [HttpPut("{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest updateRequest)
        {
            if (updateRequest == null)
            {
                return BadRequest(new ResponseObject<bool>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Dữ liệu cập nhật không hợp lệ.",
                    Data = false
                });
            }

            var result = await _userService.UpdateUserAsync(userId, updateRequest);
            return StatusCode(result.Status, result);
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var response = await _userService.GetUserInfoAsync();
            return StatusCode(response.Status, response);
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfoUpdateRequest request)
        {
            var response = await _userService.UpdateUserInfoAsync(request);
            return StatusCode(response.Status, response);
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUserDetails()
        {
            var response = await _userService.GetAllUserDetailsAsync();
            return StatusCode(response.Status, response);
        }
        [HttpPut("updateUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> UpdateRoleUserRoleName([FromBody] UpdateUserRoleRequest request)
        {
            var response = await _userService.UpdateUserRoleAsync(request.UserId, request.NewRoleId);
            return StatusCode(response.Status, response);
        }

    }
}
