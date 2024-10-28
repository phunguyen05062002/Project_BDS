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
    public interface IRoleService
    {
        Task<ResponseObject<IEnumerable<DataResponeseRole>>> GetAllRolesAsync();
        Task<ResponseObject<DataResponeseRole>> GetRoleByIdAsync(int id);
        Task<ResponseObject<string>> CreateRoleAsync(Request_Role request);
        Task<ResponseObject<string>> UpdateRoleAsync(int id, Request_Role request);
        Task<ResponseObject<string>> DeleteRoleAsync(int id);
    }
}
