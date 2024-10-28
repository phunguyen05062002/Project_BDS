using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Project_BDS.Application.HandleEmail;
using Project_BDS.Application.InterfaceService;
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
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IContactRepository _contactRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IEmailService emailService, IContactRepository contactRepository, INotificationRepository notificationRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _emailService = emailService;
            _contactRepository = contactRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendNotificationAsync(int contactId)
        {
            try
            {
                var currentUser = _httpContextAccessor.HttpContext.User;

                // Kiểm tra xem người dùng đã xác thực chưa
                if (!currentUser.Identity.IsAuthenticated)
                {
                    throw new ApplicationException("Người dùng chưa được xác thực.");
                }

                // Lấy thông tin RoleId của người dùng hiện tại
                var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                var userRoleIdClaim = currentUser.FindFirst("RoleId");
                if (userIdClaim == null || userRoleIdClaim == null)
                {
                    throw new ApplicationException("Không tìm thấy thông tin UserId hoặc RoleId trong token.");
                }

                var currentUserId = int.Parse(userIdClaim.Value);
                var currentUserRoleId = int.Parse(userRoleIdClaim.Value);

                // Kiểm tra RoleId của người dùng hiện tại
                if (currentUserRoleId != 3)
                {
                    throw new ApplicationException("Bạn không có quyền gửi thông báo.");
                }

                var contact = await _contactRepository.GetContactByIdAsync(contactId);
                if (contact == null)
                {
                    throw new ApplicationException($"Không tìm thấy yêu cầu liên hệ có ID {contactId}.");
                }

                var emailContent = "Nội dung email bạn muốn gửi cho người nhận";
                var emailSubject = "Tiêu đề email bạn muốn gửi cho người nhận";

                // Gửi email bằng EmailService
                var emailMessage = new EmailMessage(
                    to: new List<string> { "recipient@example.com" }, // Thay thế bằng email người nhận
                    subject: emailSubject,
                    content: emailContent
                );
                await _emailService.SendEmailAsync(emailMessage);

                // Tạo và lưu thông báo vào bảng Notification
                var notification = new Notification
                {
                    StatusId = 2, // Trạng thái thông báo đã xử lý
                    CreateTime = DateTime.UtcNow,
                    Content = emailContent,
                    Title = emailSubject,
                    CreateId = currentUserId,
                    Type = "Appointment", // Kiểu thông báo hẹn lịch
                    ExpiredDateTime = DateTime.UtcNow.AddDays(7), // Thông báo hết hạn sau 7 ngày
                    Code = "APPT_" + Guid.NewGuid().ToString(), // Mã thông báo
                    ContactId = contactId
                };
                await _notificationRepository.AddNotificationAsync(notification);

                // Cập nhật trạng thái yêu cầu liên hệ đã gửi thông báo
                contact.IsStatus = true; // Đã gửi thông báo hẹn lịch
                await _contactRepository.UpdateContactAsync(contact);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Đã xảy ra lỗi khi gửi thông báo hẹn lịch.", ex);
            }
        }

    }
}
