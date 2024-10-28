using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.InterfaceService
{
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseUser>> Register(Request_Register request);
        Task<string> ConfirmRegisterAccount(string confirmCode);
        Task<ResponseObject<string>> ResendConfirmationCode(string email);
        Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user);
        Task<ResponseObject<DataResponseLogin>> Login(Request_Login request);
        Task<ResponseObject<DataResponseUser>> ChangePassword(int userId, Request_ChangePassword request);
        Task<ResponseObject<string>> ForgotPassword(Request_ForgotPassword request);
        Task<ResponseObject<string>> ResetPassword(Request_ResetPassword request);
        Task<ResponseObject<string>> Logout(string refreshToken);
        //Task Logout(int userId);
    }
}
