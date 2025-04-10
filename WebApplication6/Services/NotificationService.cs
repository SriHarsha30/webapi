using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<Notification> GetAllNotifications()
        {
            return _notificationRepository.GetAllNotifications();

        }

        public Notification GetNotificationById(int id)
        {
            var notification = _notificationRepository.GetNotificationById(id);
            if (notification == null)
            {
                throw new KeyNotFoundException($"Notification with ID {id} not found.");
            }
            return notification;
        }

        public IEnumerable<Notification> GetNotificationsBySenderId(string senderId)
        {
            if (string.IsNullOrWhiteSpace(senderId))
            {
                throw new ArgumentException("Sender ID cannot be null or empty.");
            }
            return _notificationRepository.GetNotificationsBySenderId(senderId);
        }

        public void AddNotification(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification cannot be null.");
            }
            _notificationRepository.AddNotification(notification);
        }

        public void UpdateNotification(Notification notification)
        {
            if (_notificationRepository.GetNotificationById(notification.Notification_Id) == null)
            {
                throw new KeyNotFoundException($"Notification with ID {notification.Notification_Id} not found.");
            }
            _notificationRepository.UpdateNotification(notification);
        }

        public void DeleteNotification(int id)
        {
            if (_notificationRepository.GetNotificationById(id) == null)
            {
                throw new KeyNotFoundException($"Notification with ID {id} not found.");
            }
            _notificationRepository.DeleteNotification(id);
        }
    }
}
