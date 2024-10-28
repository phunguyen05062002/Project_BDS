using Project_BDS.Application.InterfaceService;
using Microsoft.Extensions.Configuration;
using Project_BDS.Application.Payloads.Mappers;
using Project_BDS.Application.Payloads.RequestModels.UserRequests;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Project_BDS.Domain.Validation;
using System.Net.Mail;
using BCryptNet = BCrypt.Net.BCrypt;
using NETCore.MailKit.Core;
using Project_BDS.Application.HandleEmail;
using IEmailService = Project_BDS.Application.InterfaceService.IEmailService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Project_BDS.Application.ImplementService
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _confirmEmailRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IBaseRepository<Notification> _baseNotificationRepository;
        public AuthService(IBaseRepository<User> baseUserRepository, UserConverter userConverter,
            IConfiguration configuration, IUserRepository userRepository,
            IEmailService emailService,
             IBaseRepository<ConfirmEmail> confirmEmailRepository, IBaseRepository<RefreshToken> baseRefreshTokenRepository,
             IBaseRepository<Notification> baseNotificationRepository)
        {
            _baseUserRepository = baseUserRepository;
            _userConverter = userConverter;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _confirmEmailRepository = confirmEmailRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _baseNotificationRepository = baseNotificationRepository;
        }

        #region Đăng ký tài khoản
        public async Task<ResponseObject<DataResponseUser>> Register(Request_Register request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName)
                    || string.IsNullOrWhiteSpace(request.Password)
                    || string.IsNullOrWhiteSpace(request.Email)
                    || string.IsNullOrWhiteSpace(request.PhoneNumber)
                    || string.IsNullOrWhiteSpace(request.FullName))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Vui lòng điền đầy đủ thông tin!",
                        Data = null
                    };
                }

                if (!ValidateInput.IsValidEmail(request.Email))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Định dạng email không hợp lệ!",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByEmail(request.Email) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email đã tồn tại trong hệ thống! Vui lòng sử dụng email khác!",
                        Data = null
                    };
                }

                if (!ValidateInput.IsValidPhoneNumber(request.PhoneNumber))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Định dạng số điện thoại không hợp lệ!",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByPhoneNumber(request.PhoneNumber) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Số điện thoại đã tồn tại trong hệ thống! Vui lòng sử dụng số điện thoại khác!",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByUsername(request.UserName) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tên người dùng đã tồn tại trong hệ thống! Vui lòng nhập tên khác!",
                        Data = null
                    };
                }

                var defaultUserStatusId = 1;
                var defaultUserRoleId = 4;
                var avatarMale = "https://www.google.com/imgres?q=avatar%20gi%E1%BB%9Bi%20t%C3%ADnh%20nam%20ho%E1%BA%B7c%20n%E1%BB%AF%20default&imgurl=https%3A%2F%2Flookaside.fbsbx.com%2Flookaside%2Fcrawler%2Fmedia%2F%3Fmedia_id%3D2781301268820883&imgrefurl=https%3A%2F%2Fwww.facebook.com%2Fkhongsocho.official%2Fposts%2Fn%25E1%25BA%25BFu-kh%25C3%25B4ng-mu%25E1%25BB%2591n-%25C4%2591%25E1%25BB%2583-avatar-nh%25C6%25B0ng-phi%25C3%25AAn-b%25E1%25BA%25A3n-c%25E1%25BB%25A7a-facebook-l%25E1%25BA%25A1i-qu%25C3%25A1-ch%25C3%25A1n-th%25C3%25AC-qu%25E1%25BA%25B9o-v%25C3%25B4-%25C4%2591%2F2781302358820774%2F&docid=Zy4yUD6o0H26qM&tbnid=GBMcV6J3Nx2QLM&vet=12ahUKEwi4_pyry6qHAxX3r1YBHdA9DwoQM3oECBYQAA..i&w=720&h=720&hcb=2&ved=2ahUKEwi4_pyry6qHAxX3r1YBHdA9DwoQM3oECBYQAA";
                var avatarFeMale = "https://www.google.com/imgres?q=avatar%20gi%E1%BB%9Bi%20t%C3%ADnh%20nam%20ho%E1%BA%B7c%20n%E1%BB%AF%20default&imgurl=https%3A%2F%2Fs3v2.interdata.vn%3A9000%2Fs3-586-15343-storage%2Fdienthoaigiakho%2Fwp-content%2Fuploads%2F2024%2F01%2F31134004%2Favatar-nam-nu-trang-8.jpg&imgrefurl=https%3A%2F%2Fdienthoaigiakho.vn%2Ftin-cong-nghe%2Fsc1400-anh-avatar-facebook-trang%2F&docid=zpwGsbY9W8Qk1M&tbnid=50z_9E_PhjXQQM&vet=12ahUKEwi4_pyry6qHAxX3r1YBHdA9DwoQM3oECHIQAA..i&w=450&h=450&hcb=2&ved=2ahUKEwi4_pyry6qHAxX3r1YBHdA9DwoQM3oECHIQAA";

                var newUser = new User
                {
                    Gender = request.Gender,
                    IsActive = true,
                    DateOfBirth = request.DateOfBirth,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Password = BCryptNet.HashPassword(request.Password),
                    UserName = request.UserName,
                    RoleId = defaultUserRoleId,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    StatusId = defaultUserStatusId,
                    Avatar = request.Gender ? avatarMale : avatarFeMale,
                };

                newUser = await _baseUserRepository.CreateAsync(newUser);

                // Tạo mới mã xác nhận email
                var confirmEmail = new ConfirmEmail
                {
                    Code = GenerateCodeActive(),
                    RequiredDateTime = DateTime.Now,
                    ExpiredDateTime = DateTime.Now.AddMinutes(20), // Thời gian hết hạn 2 phút
                    IsConfirm = false,
                    UserId = newUser.Id,
                };

                confirmEmail = await _confirmEmailRepository.CreateAsync(confirmEmail);
                var message = new EmailMessage(new string[] { request.Email }, "Nhận mã xác nhận tại đây: ", $"Mã xác nhận: {confirmEmail.Code}");
                var responseMessage = _emailService.SendEmail(message);

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Bạn đã gửi yêu cầu đăng ký! Vui lòng nhận mã xác nhận tại email để đăng ký tài khoản.",
                    Data = _userConverter.EntityToDTO(newUser)
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

        #region Phương thức xác thực Email
        private string GenerateCodeActive()
        {
            return "VanPhu_" + DateTime.Now.Ticks.ToString();
        }
        #endregion

        #region Xác nhận đăng ký tài khoản
        public async Task<string> ConfirmRegisterAccount(string confirmCode)
        {
            try
            {
                var code = await _confirmEmailRepository.GetAsync(x => x.Code.Equals(confirmCode));
                if (code == null)
                {
                    return "Mã xác nhận không hợp lệ!";
                }
                if (code.ExpiredDateTime < DateTime.Now)
                {
                    return "Mã xác nhận đã hết hạn!";
                }

                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if (user == null)
                {
                    return "Người dùng không tồn tại!";
                }
                code.IsConfirm = true;
                await _confirmEmailRepository.UpdateAsync(code);

                user.StatusId = 2;
                await _baseUserRepository.UpdateAsync(user);
                return "Xác nhận đăng ký tài khoản thành công. Bạn có thể đăng nhập và sử dụng dịch vụ.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Gửi lại mã xác nhận khi mã hết hạn
        public async Task<ResponseObject<string>> ResendConfirmationCode(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người dùng không tồn tại!",
                        Data = null
                    };
                }

                var existingCode = await _confirmEmailRepository.GetAsync(x => x.UserId == user.Id && x.ExpiredDateTime > DateTime.Now);
                if (existingCode != null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Hiện tại vẫn còn mã xác nhận hợp lệ!",
                        Data = null
                    };
                }

                await _confirmEmailRepository.DeleteAsync(x => x.UserId == user.Id);

                var newConfirmCode = new ConfirmEmail
                {
                    Code = GenerateCodeActive(),
                    RequiredDateTime = DateTime.Now,
                    ExpiredDateTime = DateTime.Now.AddMinutes(2),
                    IsConfirm = false,
                    UserId = user.Id,
                };

                await _confirmEmailRepository.CreateAsync(newConfirmCode);

                var message = new EmailMessage(new string[] { email }, "Nhận mã xác nhận tại đây: ", $"Mã xác nhận: {newConfirmCode.Code}");

                // Gọi phương thức bất đồng bộ
                await _emailService.SendEmailAsync(message);

                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đã gửi lại mã xác nhận thành công. Vui lòng kiểm tra email của bạn.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi gửi lại mã xác nhận: " + ex.Message,
                    Data = null
                };
            }
        }

        #endregion

        #region Tạo access token
        public async Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user)
        {
            var jwtSecretKey = _configuration["JWT:SecretKey"];
            var tokenValidityStr = _configuration["JWT:TokenValidityInHours"];
            var refreshTokenValidityStr = _configuration["JWT:RefreshTokenValidity"];

            if (string.IsNullOrEmpty(jwtSecretKey))
            {
                throw new ArgumentNullException("JWT:SecretKey", "JWT secret key không thể rỗng hoặc trống.");
            }

            if (!int.TryParse(tokenValidityStr, out int tokenValidityInHours))
            {
                throw new ArgumentException("Access token không hợp lệ!");
            }

            if (!int.TryParse(refreshTokenValidityStr, out int refreshTokenValidity))
            {
                throw new ArgumentException("Refresh token không hợp lệ!");
            }

            var authClaims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Sử dụng claim chuẩn Sub cho User ID
        new Claim("Id", user.Id.ToString()), // Thêm claim này
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("RoleId", user.RoleId.ToString()) // Thêm RoleId nếu cần
    };

            var jwtToken = GetToken(authClaims, jwtSecretKey, tokenValidityInHours);
            var refreshToken = GenerateRefreshToken();

            var rf = new RefreshToken
            {
                ExpiredTime = DateTime.Now.AddHours(refreshTokenValidity),
                UserId = user.Id,
                Token = refreshToken
            };

            await _baseRefreshTokenRepository.CreateAsync(rf);

            return new ResponseObject<DataResponseLogin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tạo token thành công.",
                Data = new DataResponseLogin
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = refreshToken,
                }
            };
        }
        #endregion

        #region private mothod
        private JwtSecurityToken GetToken(List<Claim> authClaims, string jwtSecretKey, int tokenValidityInHours)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var expirationUTC = DateTime.Now.AddHours(tokenValidityInHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationUTC,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        #endregion

        #region Tạo refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
        #endregion

        #region Đăng nhập tài khoản
        public async Task<ResponseObject<DataResponseLogin>> Login(Request_Login request)
        {
            var user = await _baseUserRepository.GetAsync(x => x.UserName.Equals(request.UserName));
            if (user == null)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Tài khoản không tồn tại!",
                    Data = null
                };
            }

            if (user.StatusId != 2)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Tài khoản chưa được xác thực!",
                    Data = null
                };
            }

            bool checkPass = BCryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu không chính xác!",
                    Data = null
                };
            }
            var tokenResponse = await GetJwtTokenAsync(user);
            if (tokenResponse.Status != StatusCodes.Status200OK)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = tokenResponse.Status,
                    Message = "Đăng nhập thành công, nhưng lỗi khi tạo token: " + tokenResponse.Message,
                    Data = null
                };
            }

            return new ResponseObject<DataResponseLogin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đăng nhập thành công!",
                Data = new DataResponseLogin
                {
                    AccessToken = tokenResponse.Data.AccessToken,
                    RefreshToken = tokenResponse.Data.RefreshToken
                }
            };
        }
        #endregion

        #region Đổi mật khẩu
        public async Task<ResponseObject<DataResponseUser>> ChangePassword(int userId, Request_ChangePassword request)
        {
            // Lấy thông tin user từ repository
            var user = await _baseUserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            // Xác thực mật khẩu cũ
            if (!BCryptNet.Verify(request.OldPassword, user.Password))
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu cũ không chính xác!",
                    Data = null
                };
            }

            // Kiểm tra mật khẩu mới có khớp với mật khẩu xác nhận
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu mới và mật khẩu xác nhận không khớp!",
                    Data = null
                };
            }

            // Cập nhật mật khẩu mới cho người dùng
            user.Password = BCryptNet.HashPassword(request.NewPassword);
            await _baseUserRepository.UpdateAsync(user);

            return new ResponseObject<DataResponseUser>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đổi mật khẩu thành công.",
                Data = _userConverter.EntityToDTO(user)
            };
        }


        #endregion

        #region Quên mật khẩu
        public async Task<ResponseObject<string>> ForgotPassword(Request_ForgotPassword request)
        {
            try
            {
                // Kiểm tra xem email có tồn tại không
                var user = await _baseUserRepository.GetAsync(x => x.Email.Equals(request.Email));
                if (user == null)
                {
                    // Trả về lỗi cụ thể nếu email không tồn tại
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email không tồn tại trong hệ thống!",
                        Data = null
                    };
                }

                // Đoạn code tạo thông báo và gửi email
                var resetCode = GenerateResetPasswordCode();
                var notification = new Notification
                {
                    CreateId = user.Id,
                    CreateTime = DateTime.Now,
                    Content = $"Mã đặt lại mật khẩu của bạn là: {resetCode}",
                    Title = "Yêu cầu đặt lại mật khẩu",
                    StatusId = 1,
                    Type = "ResetPassword",
                    ExpiredDateTime = DateTime.Now.AddMinutes(10),
                    Code = resetCode,
                    ContactId = null
                };

                await _baseNotificationRepository.CreateAsync(notification);

                var message = new EmailMessage(new string[] { request.Email }, notification.Title, notification.Content);
                _emailService.SendEmail(message);

                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Yêu cầu đặt lại mật khẩu đã được gửi! Vui lòng kiểm tra email của bạn.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // Trả về lỗi hệ thống nếu có ngoại lệ
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    Data = null
                };
            }
        }




        private string GenerateResetPasswordCode()
        {
            return "Reset_" + DateTime.Now.Ticks.ToString();
        }

        #endregion

        #region Xác nhận mã đặt lại mật khẩu và tạo mật khẩu mới
        public async Task<ResponseObject<string>> ResetPassword(Request_ResetPassword request)
        {
            // Kiểm tra thông báo đặt lại mật khẩu
            var notification = await _baseNotificationRepository.GetAsync(x => x.Code == request.Code && x.Type == "ResetPassword");

            // Kiểm tra mã đặt lại mật khẩu không hợp lệ
            if (notification == null)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mã đặt lại mật khẩu không hợp lệ!",
                    Data = null
                };
            }

            // Kiểm tra mã đã hết hạn
            if (notification.ExpiredDateTime < DateTime.UtcNow)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mã đặt lại mật khẩu đã hết hạn!",
                    Data = null
                };
            }

            // Kiểm tra người dùng
            var user = await _baseUserRepository.GetAsync(x => x.Id == notification.CreateId);
            if (user == null)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng không tồn tại!",
                    Data = null
                };
            }

            // Kiểm tra mật khẩu mới và mật khẩu xác nhận có khớp không
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu mới và mật khẩu xác nhận không khớp!",
                    Data = null
                };
            }

            // Cập nhật mật khẩu người dùng
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            try
            {
                await _baseUserRepository.UpdateAsync(user);
            }
            catch (Exception)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi khi cập nhật mật khẩu. Vui lòng thử lại sau.",
                    Data = null
                };
            }

            // Cập nhật trạng thái thông báo
            notification.StatusId = 2; // Đã xử lý
            try
            {
                await _baseNotificationRepository.UpdateAsync(notification);
            }
            catch (Exception)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái thông báo. Vui lòng thử lại sau.",
                    Data = null
                };
            }

            return new ResponseObject<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Mật khẩu đã được đặt lại thành công.",
                Data = null
            };
        }


        #endregion

        #region Đăng xuất
        public async Task<ResponseObject<string>> Logout(string refreshToken)
        {
            try
            {
                var token = await _baseRefreshTokenRepository.GetAsync(rt => rt.Token == refreshToken);

                if (token == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Refresh token không hợp lệ hoặc không tồn tại!",
                        Data = null
                    };
                }

                await _baseRefreshTokenRepository.DeleteAsync(t => t.Token == refreshToken);

                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đăng xuất thành công!",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi khi đăng xuất: " + ex.Message,
                    Data = null
                };
            }
        }
        #endregion

    }
}
