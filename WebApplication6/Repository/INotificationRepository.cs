using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface INotificationRepository
    {
        IEnumerable<Notification> GetAllNotifications();
        Notification GetNotificationById(int id);
        IEnumerable<Notification> GetNotificationsBySenderId(string senderId);
        void AddNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(int id);
    }
}
