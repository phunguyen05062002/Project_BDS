using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.DataContexts;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.ImplementRepostories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Xu li chuoi
        private Task<bool> CompareStringAsync(string str1, string str2)
        {
            return Task.FromResult(string.Equals(str1.ToLowerInvariant(), str2.ToLowerInvariant()));
        }
        #endregion

        public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role) // Bao gồm Role để có thông tin RoleName
            .ToListAsync();
    }


        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.UserName == username);
        }


        public async Task AddRoleToUserAsync(User user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var roleItem = _context.Roles.SingleOrDefaultAsync(x => x.RoleCode.Equals(role));
            if (roleItem == null)
            {
                throw new InvalidOperationException("Quyền này không tồn tại!");
            }
            user.RoleId = roleItem.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleOfUserAsync(User user, string newRole)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (newRole == null)
            {
                throw new ArgumentNullException(nameof(newRole));
            }

            var roleItem = await _context.Roles.SingleOrDefaultAsync(x => x.RoleCode.Equals(newRole));
            if (roleItem == null)
            {
                throw new InvalidOperationException("Không tồn tại quyền này!");
            }

            user.RoleId = roleItem.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetRoleOfUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại!");
            }
            var role = await _context.Roles.Where(r => r.Id == user.RoleId).Select(r => r.RoleCode).SingleOrDefaultAsync();
            return role;
        }
        public async Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            return _context.Users.Where(predicate).AsQueryable();

        }

        public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId)
        {
            return await _context.Users.Where(u => u.RoleId == roleId).ToListAsync();
        }
        public async Task<IEnumerable<UserDto>> GetUserDetailsAsync()
        {
            return await _context.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId).RoleName
                })
                .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
