using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MvcWebApp.Hubs;

namespace MvcWebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationsController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // Client'lar ile iletişim. fuction erişebileceği bir entpoint
        // .../CompleteWatermarkProcess/1554351315    bu connectionId yi Url'de almak için HttpGet'e connectionId yazmak gerekir
        [HttpGet("{connectionId}")]
        public IActionResult CompleteWatermarkProcess(string connectionId)//Tamamlanma olayı
        {
            _hubContext.Clients.Client(connectionId).SendAsync("notifyCompleteWatermarkProcess");//Client buraya subsc. olduysa herhangi bir data göndermicem

            return Ok();
        }
    }
}
