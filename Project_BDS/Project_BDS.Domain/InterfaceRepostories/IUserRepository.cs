using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetUserDetailsAsync();
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task AddRoleToUserAsync(User user, string role);
        Task UpdateRoleOfUserAsync(User user, string newRole);
        Task<string> GetRoleOfUserAsync(int id);
        Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate);
        Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
        Task UpdateAsync(User user);
    }
}
