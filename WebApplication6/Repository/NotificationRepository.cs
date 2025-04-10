using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly Context _context;

        public NotificationRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Notification> GetAllNotifications()
        {
            return _context.notifications1.ToList();
        }

        public Notification GetNotificationById(int id)
        {
            int a = id;
            return _context.notifications1.SingleOrDefault(n => n.Notification_Id == a);
        }

        public IEnumerable<Notification> GetNotificationsBySenderId(string senderId)
        {
            return _context.notifications1.Where(n => n.sendersId == senderId || n.receiversId == senderId).ToList();
        }

        public void AddNotification(Notification notification)
        {
            if (notification.CreatedDate == DateTime.MinValue)
            {
                notification.CreatedDate = DateTime.UtcNow; // Set current timestamp if not provided
            }
            _context.notifications1.Add(notification);
            _context.SaveChanges();
        }

        public void UpdateNotification(Notification notification)
        {
            _context.notifications1.Update(notification);
            _context.SaveChanges();
        }

        public void DeleteNotification(int id)
        {
            var notification = GetNotificationById(id);
            if (notification != null)
            {
                _context.notifications1.Remove(notification);
                _context.SaveChanges();
            }
        }
    }
}
