using Microsoft.AspNetCore.Http;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.Mappers;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.ImplementService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository<User> _baseUserRepository;

        public RoleService(IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor, IBaseRepository<User> baseUserRepository)
        {
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
            _baseUserRepository = baseUserRepository;
        }

        private async Task<ResponseObject<T>> ValidateCurrentUser<T>()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Người dùng chưa được xác thực!",
                    Data = default(T)
                };
            }

            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin UserId trong token!",
                    Data = default(T)
                };
            }

            var currentUserId = int.Parse(userIdClaim.Value);
            var currentUserInfo = await _baseUserRepository.GetByIdAsync(currentUserId);
            if (currentUserInfo == null)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
                    Data = default(T)
                };
            }

            var currentUserRoleId = currentUserInfo.RoleId;

            if (currentUserRoleId != 1)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền truy cập vào chức năng này.",
                    Data = default(T)
                };
            }

            return null;
        }

        public async Task<ResponseObject<IEnumerable<DataResponeseRole>>> GetAllRolesAsync()
        {
            var validationResponse = await ValidateCurrentUser<IEnumerable<DataResponeseRole>>();
            if (validationResponse != null)
                return validationResponse;

            var roles = await _roleRepository.GetAllRolesAsync();
            var response = roles.Select(r => RoleConverter.EntityToDTO(r)).ToList();
            return new ResponseObject<IEnumerable<DataResponeseRole>>(200, "Success", response);
        }

        public async Task<ResponseObject<DataResponeseRole>> GetRoleByIdAsync(int id)
        {
            var validationResponse = await ValidateCurrentUser<DataResponeseRole>();
            if (validationResponse != null)
                return validationResponse;

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                return new ResponseObject<DataResponeseRole>(404, "Role not found", null);

            return new ResponseObject<DataResponeseRole>(200, "Success", RoleConverter.EntityToDTO(role));
        }

        public async Task<ResponseObject<string>> CreateRoleAsync(Request_Role request)
        {
            var validationResponse = await ValidateCurrentUser<string>();
            if (validationResponse != null)
                return validationResponse;

            var newRole = new Role
            {
                RoleCode = request.RoleCode,
                RoleName = request.RoleName
            };

            await _roleRepository.AddRoleAsync(newRole);
            return new ResponseObject<string>(201, "Role created successfully", "Role created");
        }

        public async Task<ResponseObject<string>> UpdateRoleAsync(int id, Request_Role request)

        {
            var validationResponse = await ValidateCurrentUser<string>();
            if (validationResponse != null)
                return validationResponse;

            var existingRole = await _roleRepository.GetRoleByIdAsync(id);
            if (existingRole == null)
                return new ResponseObject<string>(404, "Role not found", "Role not found");

            existingRole.RoleCode = request.RoleCode;
            existingRole.RoleName = request.RoleName;

            await _roleRepository.UpdateRoleAsync(existingRole);
            return new ResponseObject<string>(200, "Role updated successfully", "Role updated");
        }

        public async Task<ResponseObject<string>> DeleteRoleAsync(int id)
        {
            var validationResponse = await ValidateCurrentUser<string>();
            if (validationResponse != null)
                return validationResponse;

            var existingRole = await _roleRepository.GetRoleByIdAsync(id);
            if (existingRole == null)
                return new ResponseObject<string>(404, "Role not found", "Role not found");

            await _roleRepository.DeleteRoleAsync(id);
            return new ResponseObject<string>(200, "Role deleted successfully", "Role deleted");
        }
    }
}
