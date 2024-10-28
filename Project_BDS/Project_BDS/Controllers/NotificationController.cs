using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.InterfaceService;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost("send/{contactId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendNotification(int contactId)
        {
            await _notificationService.SendNotificationAsync(contactId);
            return Ok(new { message = "Thông báo đã được gửi thành công!" });
        }
    }
}
