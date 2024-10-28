using Project_BDS.Application.Helper;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.InterfaceService
{
    public interface IUserService
    {
        Task<ResponseObject<DataResponseUser>> UpdateUserRole(int adminId, UpdateUserRoleRequest request);
        Task<ResponseObject<DataResponseUser>> UpdateRoleUser(UpdateUserRoleRequest request);
        //Task<ResponseObject<PagedResult<DataResponseUser>>> GetAllUsers(int pageSize, int pageNumber);
        Task<ResponseObject<IEnumerable<DataResponseUser>>> GetAllUsers();
        Task<ResponseObject<int>> CountUsers();
        Task<ResponseObject<DataResponseUser>> GetUserByEmail(string email);
        Task<ResponseObject<List<DataResponseUser>>> GetUsersByFullName(string fullName, int pageNumber, int pageSize);
        Task<ResponseObject<DataResponseUser>> GetUserByPhoneNumber(string phoneNumber);
        Task<ResponseObject<bool>> DeleteUserAsync(int userId);
        Task<ResponseObject<bool>> UpdateUserAsync(int userId, UpdateUserRequest updateRequest);
        Task<ResponseObject<DataResponse_UserInfo>> GetUserInfoAsync();
        Task<ResponseObject<bool>> UpdateUserInfoAsync(UserInfoUpdateRequest request);
        Task<ResponseObject<IEnumerable<UserDto>>> GetAllUserDetailsAsync();
        Task<ResponseObject<UserDto>> UpdateUserRoleAsync(int userId, int newRoleId);
    }
}
