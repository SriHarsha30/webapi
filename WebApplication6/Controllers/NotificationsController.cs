using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Services;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationsController : ControllerBase

    {

        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)

        {

            _notificationService = notificationService;

        }

        // GET: api/Notifications

        [HttpGet]
        [Authorize(Roles = "o,t")]
        public ActionResult<IEnumerable<Notification>> GetAllNotifications()

        {

            var notifications = _notificationService.GetAllNotifications();

            if (notifications == null || !notifications.Any())

            {

                return NotFound("No notifications found.");

            }

            return Ok(notifications);

        }

        // GET: api/Notifications/{id}

        [HttpGet("{id}")]
        [Authorize(Roles = "o,t")]

        public ActionResult<Notification> GetNotificationById(int id)

        {

            try

            {

                var notification = _notificationService.GetNotificationById(id);

                return Ok(notification);

            }

            catch (KeyNotFoundException ex)

            {

                return NotFound(ex.Message);

            }

        }

        // GET: api/Notifications/ByUID/{senderId}

        [HttpGet("ByUID/{senderId}")]
        [Authorize(Roles = "o,t")]
        public ActionResult<IEnumerable<Notification>> GetNotificationsBySender(string senderId)

        {

            try

            {

                var notifications = _notificationService.GetNotificationsBySenderId(senderId);

                if (notifications == null || !notifications.Any())

                {

                    return NotFound($"No notifications found for sender ID {senderId}.");

                }

                return Ok(notifications);

            }

            catch (System.ArgumentException ex)

            {

                return BadRequest(ex.Message);

            }

        }

        // POST: api/Notifications

        [HttpPost]
        [Authorize(Roles = "o,t")]
        public ActionResult<Notification> PostNotification(Notification notification)

        {

            if (notification == null)

            {

                return BadRequest("Notification cannot be null.");

            }

            if (string.IsNullOrWhiteSpace(notification.sendersId) || string.IsNullOrWhiteSpace(notification.receiversId))

            {

                return BadRequest("Sender ID and Receiver ID are required.");

            }

            try

            {

                _notificationService.AddNotification(notification);

                return CreatedAtAction(nameof(GetNotificationById),

                    new { id = notification.Notification_Id },

                    notification);

            }

            catch (System.ArgumentNullException ex)

            {

                return BadRequest(ex.Message);

            }

        }

        // PUT: api/Notifications/{id}

        [HttpPut("{id}")]
        [Authorize(Roles = "o,t")]
        public IActionResult PutNotification(int id, Notification notification)

        {

            if (id != notification.Notification_Id)

            {

                return BadRequest("Notification ID mismatch.");

            }

            try

            {

                _notificationService.UpdateNotification(notification);

                return NoContent();

            }

            catch (KeyNotFoundException ex)

            {

                return NotFound(ex.Message);

            }

        }

        // DELETE: api/Notifications/{id}

        [HttpDelete("{id}")]
        [Authorize(Roles = "o,t")]
        public IActionResult DeleteNotification(int id)

        {

            try

            {

                _notificationService.DeleteNotification(id);

                return NoContent();

            }

            catch (KeyNotFoundException ex)

            {

                return NotFound(ex.Message);

            }

        }

    }

}
