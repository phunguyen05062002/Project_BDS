using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Project_BDS.Application.Helper;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.Mappers;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project_BDS.Application.ImplementService
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserConverter _userConverter;
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IBaseRepository<User> baseUserRepository, UserConverter userConverter, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _baseUserRepository = baseUserRepository;
            _userConverter = userConverter;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ResponseObject<DataResponseUser>> UpdateRoleUser(UpdateUserRoleRequest request)
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            try
            {
                // Kiểm tra xem người dùng đã xác thực chưa
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Người dùng chưa được xác thực!",
                        Data = null
                    };
                }

                // Lấy thông tin người dùng hiện tại từ token
                var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Không tìm thấy thông tin UserId trong token!",
                        Data = null
                    };
                }

                var currentUserId = int.Parse(userIdClaim.Value);
                var currentUserInfo = await _baseUserRepository.GetByIdAsync(currentUserId);
                if (currentUserInfo == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
                        Data = null
                    };
                }

                // Lấy RoleId của người dùng hiện tại
                var currentUserRoleId = currentUserInfo.RoleId;

                // Kiểm tra UserId truyền vào
                if (request.UserId <= 0)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"UserId không hợp lệ! Giá trị hiện tại là: {request.UserId}. Phải là số dương.",
                        Data = null
                    };
                }

                // Lấy thông tin người dùng cần cập nhật
                var userToUpdate = await _baseUserRepository.GetByIdAsync(request.UserId);
                if (userToUpdate == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Người dùng không tồn tại!",
                        Data = null
                    };
                }

                // Kiểm tra quyền thay đổi RoleId
                if (currentUserRoleId == 1) // RoleId 1 là Admin
                {
                    // Admin có thể thay đổi thành Manager (2), Staff (3), hoặc User (4)
                    if (request.NewRoleId == 2 || request.NewRoleId == 3 || request.NewRoleId == 4)
                    {
                        userToUpdate.RoleId = request.NewRoleId;
                    }
                    else
                    {
                        return new ResponseObject<DataResponseUser>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "NewRoleId không hợp lệ! Phải là 2 (Manager), 3 (Staff), hoặc 4 (User).",
                            Data = null
                        };
                    }
                }
                else if (currentUserRoleId == 2) // RoleId 2 là Manager
                {
                    // Manager chỉ có thể thay đổi RoleId thành Staff (3) hoặc User (4)
                    if (request.NewRoleId == 3 || request.NewRoleId == 4)
                    {
                        userToUpdate.RoleId = request.NewRoleId;
                    }
                    else
                    {
                        return new ResponseObject<DataResponseUser>
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Message = "Bạn không có quyền thực hiện chức năng này!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Bạn không có quyền thực hiện chức năng này!",
                        Data = null
                    };
                }

                // Cập nhật RoleId cho người dùng
                await _baseUserRepository.UpdateAsync(userToUpdate);

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Cập nhật RoleId thành công.",
                    Data = _userConverter.EntityToDTO(userToUpdate)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }



        #region Cập nhật roleId
        public async Task<ResponseObject<DataResponseUser>> UpdateUserRole(int adminId, UpdateUserRoleRequest request)
        {
            try
            {
                // Kiểm tra xem người dùng hiện tại có tồn tại
                var adminUser = await _baseUserRepository.GetByIdAsync(adminId);
                if (adminUser == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Admin không tồn tại!",
                        Data = null
                    };
                }

                // Lấy thông tin người dùng cần thay đổi RoleId
                var user = await _baseUserRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Người dùng không tồn tại!",
                        Data = null
                    };
                }

                // Kiểm tra quyền hạn của admin hoặc manager
                if (adminUser.RoleId == 1) // Admin
                {
                    // Admin có thể thay đổi thành bất kỳ RoleId nào
                    user.RoleId = request.NewRoleId;
                }
                else if (adminUser.RoleId == 2) // Manager
                {
                    // Manager chỉ có thể thay đổi RoleId thành Staff (3) hoặc User (4)
                    if (request.NewRoleId == 3 || request.NewRoleId == 4)
                    {
                        user.RoleId = request.NewRoleId;
                    }
                    else
                    {
                        return new ResponseObject<DataResponseUser>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Manager chỉ có thể thay đổi RoleId thành Staff hoặc User.",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Bạn không có quyền thay đổi RoleId của người dùng khác!",
                        Data = null
                    };
                }

                // Cập nhật RoleId cho người dùng
                await _baseUserRepository.UpdateAsync(user);

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Cập nhật RoleId thành công.",
                    Data = _userConverter.EntityToDTO(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Error: " + ex.Message,
                    Data = null
                };
            }
        }
        #endregion

        //#region Lấy danh sách người dùng
        //public async Task<ResponseObject<List<DataResponseUser>>> GetAllUsers(int pageSize, int pageNumber)
        //{
        //    var currentUser = _httpContextAccessor.HttpContext.User;

        //    // Kiểm tra xem người dùng đã xác thực chưa
        //    if (!currentUser.Identity.IsAuthenticated)
        //    {
        //        return new ResponseObject<List<DataResponseUser>>
        //        {
        //            Status = StatusCodes.Status401Unauthorized,
        //            Message = "Người dùng chưa được xác thực!",
        //            Data = null
        //        };
        //    }

        //    // Lấy thông tin người dùng hiện tại từ token
        //    var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null)
        //    {
        //        return new ResponseObject<List<DataResponseUser>>
        //        {
        //            Status = StatusCodes.Status401Unauthorized,
        //            Message = "Không tìm thấy thông tin UserId trong token!",
        //            Data = null
        //        };
        //    }

        //    var currentUserId = int.Parse(userIdClaim.Value);
        //    var currentUserInfo = await _baseUserRepository.GetByIdAsync(currentUserId);
        //    if (currentUserInfo == null)
        //    {
        //        return new ResponseObject<List<DataResponseUser>>
        //        {
        //            Status = StatusCodes.Status401Unauthorized,
        //            Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
        //            Data = null
        //        };
        //    }

        //    // Lấy RoleId của người dùng hiện tại
        //    var currentUserRoleId = currentUserInfo.RoleId;

        //    // Kiểm tra RoleId của người dùng hiện tại (1 hoặc 2)
        //    if (currentUserRoleId != 1 && currentUserRoleId != 2)
        //    {
        //        return new ResponseObject<List<DataResponseUser>>
        //        {
        //            Status = StatusCodes.Status403Forbidden,
        //            Message = "Bạn không có quyền truy cập vào danh sách người dùng.",
        //            Data = null
        //        };
        //    }

        //    // Lấy danh sách tất cả người dùng
        //    var usersQuery = await _baseUserRepository.GetAllAsync();

        //    var users = usersQuery
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(user => _userConverter.EntityToDTO(user))
        //        //.Select(user => new DataResponseUser
        //        //{
        //        //    Id = user.Id,
        //        //    Email = user.Email,
        //        //    FullName = user.FullName,
        //        //    PhoneNumber = user.PhoneNumber,
        //        //    Avatar = user.Avatar,
        //        //    DateOfBirth = user.DateOfBirth,
        //        //    CreateTime = user.CreateTime,
        //        //    UpdateTime = user.UpdateTime,
        //        //    RoleId = user.RoleId,
        //        //    StatusId = user.StatusId,
        //        //    IsActive = user.IsActive
        //        //})
        //        .ToList();

        //    // Đếm tổng số người dùng
        //    var totalUsers = await _baseUserRepository.CountAsync();
        //    return new ResponseObject<List<DataResponseUser>>
        //    {
        //        Status = StatusCodes.Status200OK,
        //        Message = "Lấy danh sách người dùng thành công.",
        //        Data = users
        //    };
        //}
        //#endregion

        #region Xác thực người dùng hiện tại
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
                    Message = "Bạn không có quyền truy cập vào danh sách người dùng.",
                    Data = default(T)
                };
            }

            return null;
        }
        #endregion

        //#region Lấy danh sách người dùng
        //public async Task<ResponseObject<PagedResult<DataResponseUser>>> GetAllUsers(int pageSize, int pageNumber)
        //{
        //    var validationResponse = await ValidateCurrentUser<PagedResult<DataResponseUser>>();
        //    if (validationResponse != null) return validationResponse;

        //    var usersQuery = await _baseUserRepository.GetAllAsync();

        //    var users = usersQuery
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(user => _userConverter.EntityToDTO(user))
        //        .ToList();

        //    return new ResponseObject<PagedResult<DataResponseUser>>
        //    {
        //        Status = StatusCodes.Status200OK,
        //        Message = "Lấy danh sách người dùng thành công.",
        //        Data = new PagedResult<DataResponseUser>
        //        {
        //            Items = users,
        //            TotalPages = (int)Math.Ceiling(await _baseUserRepository.CountAsync() / (double)pageSize),
        //            CurrentPage = pageNumber
        //        }
        //    };
        //}
        //#endregion

        #region Lấy danh sách người dùng
        public async Task<ResponseObject<IEnumerable<DataResponseUser>>> GetAllUsers()
        {
            // Xác thực người dùng hiện tại
            var validationResponse = await ValidateCurrentUser<IEnumerable<DataResponseUser>>();
            if (validationResponse != null) return validationResponse;

            // Lấy danh sách người dùng từ repository
            var usersQuery = await _baseUserRepository.GetAllAsync();

            // Lấy tất cả vai trò từ repository
            var roles = await _roleRepository.GetAllRolesAsync();
            var roleDictionary = roles.ToDictionary(r => r.Id, r => r.RoleName);

            // Chuyển đổi người dùng thành DTO và cập nhật RoleName
            var userList = new List<DataResponseUser>();

            foreach (var user in usersQuery)
            {
                var userDto = _userConverter.EntityToDTO(user);
                userDto.RoleName = roleDictionary.ContainsKey(user.RoleId) ? roleDictionary[user.RoleId] : "Unknown";
                userList.Add(userDto);
            }

            // Trả về kết quả
            return new ResponseObject<IEnumerable<DataResponseUser>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách người dùng thành công.",
                Data = userList
            };
        }
        #endregion



        #region Đếm tổng số tài khoản
        public async Task<ResponseObject<int>> CountUsers()
        {
            var validationResponse = await ValidateCurrentUser<int>();
            if (validationResponse != null) return validationResponse;

            var totalUsers = await _baseUserRepository.CountAsync();
            return new ResponseObject<int>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy tổng số người dùng thành công.",
                Data = totalUsers
            };
        }
        #endregion

        #region Lấy thông tin người dùng theo Email
        public async Task<ResponseObject<DataResponseUser>> GetUserByEmail(string email)
        {
            var validationResponse = await ValidateCurrentUser<DataResponseUser>();
            if (validationResponse != null) return validationResponse;

            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại.",
                    Data = null
                };
            }

            return new ResponseObject<DataResponseUser>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin người dùng thành công.",
                Data = _userConverter.EntityToDTO(user)
            };
        }
        #endregion

        #region Lấy thông tin người dùng theo FullName
        public async Task<ResponseObject<List<DataResponseUser>>> GetUsersByFullName(string fullName, int pageSize, int pageNumber)
        {
            var validationResponse = await ValidateCurrentUser<List<DataResponseUser>>();
            if (validationResponse != null) return validationResponse;

            fullName = fullName?.Trim().ToLower();
            var usersQuery = await _userRepository.FindByAsync(user => user.FullName.ToLower().Contains(fullName));

            var users = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(user => _userConverter.EntityToDTO(user))
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return new ResponseObject<List<DataResponseUser>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy người dùng nào.",
                    Data = null
                };
            }

            return new ResponseObject<List<DataResponseUser>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách người dùng thành công.",
                Data = users
            };
        }
        #endregion

        #region Lấy thông tin người dùng theo PhoneNumber
        public async Task<ResponseObject<DataResponseUser>> GetUserByPhoneNumber(string phoneNumber)
        {
            var validationResponse = await ValidateCurrentUser<DataResponseUser>();
            if (validationResponse != null) return validationResponse;

            var user = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại.",
                    Data = null
                };
            }

            return new ResponseObject<DataResponseUser>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin người dùng thành công.",
                Data = _userConverter.EntityToDTO(user)
            };
        }
        #endregion

        #region Xóa người dùng 
        public async Task<ResponseObject<bool>> DeleteUserAsync(int userId)
        {
            var permissionResponse = await CheckUserPermissions(userId);
            if (permissionResponse != null) return permissionResponse;

            // Xóa người dùng
            await _baseUserRepository.DeleteAsync(userId);

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa người dùng thành công.",
                Data = true
            };
        }

        #endregion

        #region Sửa thông tin người dùng
        public async Task<ResponseObject<bool>> UpdateUserAsync(int userId, UpdateUserRequest updateRequest)
        {
            var permissionResponse = await CheckUserPermissions(userId);
            if (permissionResponse != null) return permissionResponse;

            // Lấy thông tin người dùng hiện tại từ token
            var currentUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = int.Parse(currentUserIdClaim.Value);
            var currentUser = await _baseUserRepository.GetByIdAsync(currentUserId);

            // Kiểm tra xem người dùng có tồn tại không
            var user = await _baseUserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại.",
                    Data = false
                };
            }

            // Cập nhật thông tin người dùng
            user.FullName = updateRequest.FullName ?? user.FullName;
            user.Email = updateRequest.Email ?? user.Email;
            user.PhoneNumber = updateRequest.PhoneNumber ?? user.PhoneNumber;

            // Cập nhật DateOfBirth chỉ nếu có giá trị
            if (updateRequest.DateOfBirth != default(DateTime))
            {
                user.DateOfBirth = updateRequest.DateOfBirth;
            }

            user.UpdateTime = DateTime.UtcNow;

            // Cập nhật RoleId nếu có giá trị hợp lệ
            if (updateRequest.RoleId > 0 && (currentUser.RoleId == 1 || (currentUser.RoleId == 2 && updateRequest.RoleId < 3)))
            {
                user.RoleId = updateRequest.RoleId;
            }

            await _baseUserRepository.UpdateAsync(user);

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật người dùng thành công.",
                Data = true
            };
        }
        #endregion

        #region Xác thực người dùng hiện tại
        private async Task<ResponseObject<T>> AuthenticateCurrentUser<T>()
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
            var currentUserInfo = await _userRepository.GetByIdAsync(currentUserId);
            if (currentUserInfo == null)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
                    Data = default(T)
                };
            }

            return null;
        }
        #endregion

        #region Lấy thông tin cá nhân
        public async Task<ResponseObject<DataResponse_UserInfo>> GetUserInfoAsync()
        {
            var validationResponse = await AuthenticateCurrentUser<DataResponse_UserInfo>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return new ResponseObject<DataResponse_UserInfo>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy thông tin người dùng!",
                    Data = null
                };
            }

            var userInfo = new DataResponse_UserInfo
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            };

            return new ResponseObject<DataResponse_UserInfo>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin người dùng thành công!",
                Data = userInfo
            };
        }
        #endregion

        #region Sửa thông tin cá nhân
        public async Task<ResponseObject<bool>> UpdateUserInfoAsync(UserInfoUpdateRequest request)
        {
            var validationResponse = await AuthenticateCurrentUser<bool>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy thông tin người dùng!",
                    Data = false
                };
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;

            await _baseUserRepository.UpdateAsync(user);

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật thông tin người dùng thành công!",
                Data = true
            };
        }
        #endregion

        #region Hàm kiểm tra quyền cho Admin và Manager
        private async Task<ResponseObject<bool>> CheckUserPermissions(int userId)
        {
            // Lấy thông tin người dùng hiện tại từ token
            var currentUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = int.Parse(currentUserIdClaim.Value);
            var currentUser = await _baseUserRepository.GetByIdAsync(currentUserId);

            // Kiểm tra quyền sửa/xóa người dùng
            var userToCheck = await _baseUserRepository.GetByIdAsync(userId);
            if (userToCheck == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại.",
                    Data = false
                };
            }

            // Kiểm tra quyền sửa
            if (currentUser.RoleId == 2 && userToCheck.RoleId == 1)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền sửa người dùng này.",
                    Data = false
                };
            }

            // Kiểm tra quyền xóa
            if (currentUser.RoleId == 2 && userToCheck.RoleId == 1)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền xóa người dùng này.",
                    Data = false
                };
            }

            if (currentUser.RoleId == 3 || currentUser.RoleId == 4)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền thực hiện thao tác này.",
                    Data = false
                };
            }

            return null; // Không có lỗi
        }

        #endregion

        public async Task<ResponseObject<IEnumerable<UserDto>>> GetAllUserDetailsAsync()
        {
            var validationResponse = await ValidateCurrentUser<IEnumerable<UserDto>>();
            if (validationResponse != null) return validationResponse;

            // Lấy tất cả thông tin người dùng bao gồm cả RoleId và RoleName
            var users = await _userRepository.GetUserDetailsAsync();

            return new ResponseObject<IEnumerable<UserDto>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin người dùng thành công.",
                Data = users
            };
        }

        public async Task<ResponseObject<UserDto>> UpdateUserRoleAsync(int userId, int newRoleId)
        {
            var validationResponse = await ValidateCurrentUser<UserDto>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseObject<UserDto>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Người dùng không tồn tại!",
                    Data = null
                };
            }

            var role = await _roleRepository.GetByIdAsync(newRoleId);
            if (role == null)
            {
                return new ResponseObject<UserDto>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Vai trò không tồn tại!",
                    Data = null
                };
            }

            // Cập nhật RoleId cho người dùng
            user.RoleId = role.Id;
            await _userRepository.UpdateAsync(user);

            // Tạo DTO để trả về thông tin đã cập nhật
            var updatedUser = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId, // Trả về RoleId
                RoleName = role.RoleName // Trả về RoleName tương ứng
            };

            return new ResponseObject<UserDto>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật vai trò người dùng thành công.",
                Data = updatedUser
            };
        }
    }
}
