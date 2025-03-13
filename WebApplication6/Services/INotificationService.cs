using WebApplication6.Models;

namespace WebApplication6.Services
{
    public interface INotificationService
    {
        IEnumerable<Notification> GetAllNotifications();
        Notification GetNotificationById(int id);
        IEnumerable<Notification> GetNotificationsBySenderId(string senderId);
        void AddNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(int id);
    }
}
