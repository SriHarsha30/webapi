using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface IProperty
    {
        bool Insert(Property property);
        bool DeleteProp(Property property);
        List<Property> ViewData();
        Task<Property> FindAsync(int id);
        void Update(Property property);
        Task SaveChangesAsync();
        bool Exists(int id);
        Task ExecuteSqlRawAsync(string sql, params object[] parameters);
        object GetById(int propertyId);
        IEnumerable<object> GetPropertiesByOwner(string ownerId);
    }
}
