using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
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
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactService(IContactRepository contactRepository, IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _contactRepository = contactRepository;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        public async Task<ResponseObject<Contact>> SendContactAsync(ContactRequest contactRequest)
        {
            try
            {
                // Lấy thông tin người dùng từ HttpContext
                var currentUser = _httpContextAccessor.HttpContext.User;

                // Kiểm tra xác thực người dùng và lấy UserId
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<Contact>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Người dùng chưa được xác thực!",
                        Data = null
                    };
                }

                // Lấy thông tin RoleId của người dùng hiện tại
                var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                var userRoleIdClaim = currentUser.FindFirst("RoleId");
                if (userIdClaim == null || userRoleIdClaim == null)
                {
                    return new ResponseObject<Contact>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Không tìm thấy thông tin UserId hoặc RoleId trong token!",
                        Data = null
                    };
                }

                var currentUserId = int.Parse(userIdClaim.Value);
                var currentUserRoleId = int.Parse(userRoleIdClaim.Value);

                // Kiểm tra RoleId của người dùng hiện tại
                if (currentUserRoleId != 4)
                {
                    return new ResponseObject<Contact>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Bạn không có quyền gửi yêu cầu liên hệ.",
                        Data = null
                    };
                }

                // Kiểm tra xem ProductId có tồn tại không
                var productExists = await _productRepository.ExistsAsync(contactRequest.ProductId);
                if (!productExists)
                {
                    return new ResponseObject<Contact>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sản phẩm không tồn tại!",
                        Data = null
                    };
                }

                // Tạo mới một thực thể Contact
                var contact = new Contact
                {
                    ProductId = contactRequest.ProductId,
                    CreateTime = DateTime.UtcNow,
                    IsStatus = false // Chưa gửi
                };

                // Lưu Contact vào repository
                await _contactRepository.AddContactAsync(contact);

                return new ResponseObject<Contact>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đã gửi yêu cầu liên hệ thành công!",
                    Data = contact
                };
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                return new ResponseObject<Contact>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<List<Contact>>> GetContactsForRoleId3Async()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            // Kiểm tra xem người dùng đã xác thực chưa
            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<List<Contact>>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Người dùng chưa được xác thực.",
                    Data = null
                };
            }

            // Lấy thông tin RoleId của người dùng hiện tại
            var userRoleIdClaim = currentUser.FindFirst("RoleId");
            if (userRoleIdClaim == null)
            {
                return new ResponseObject<List<Contact>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không tìm thấy thông tin RoleId trong token.",
                    Data = null
                };
            }

            var currentUserRoleId = int.Parse(userRoleIdClaim.Value);

            // Kiểm tra RoleId của người dùng hiện tại
            if (currentUserRoleId != 3)
            {
                return new ResponseObject<List<Contact>>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền truy cập danh sách liên hệ.",
                    Data = null
                };
            }

            var contacts = await _contactRepository.GetAllContactsAsync();

            return new ResponseObject<List<Contact>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách liên hệ thành công.",
                Data = contacts
            };
        }


    }
}
