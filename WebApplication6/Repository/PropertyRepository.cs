using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public class PropertyRepository : IProperty
    {
        private readonly Context _context;

        public PropertyRepository(Context context)
        {
            _context = context;
        }

        public bool Insert(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteProp(Property property)
        {
            var existingProperty = _context.Properties.Find(property.Property_Id);
            if (existingProperty != null)
            {
                _context.Properties.Remove(existingProperty);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Property> ViewData()
        {
            return _context.Properties.ToList();
        }

        public async Task<Property> FindAsync(int id)
        {
            return await _context.Properties.FindAsync(id);
        }

        public void Update(Property property)
        {
            _context.Entry(property).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Properties.Any(e => e.Property_Id == id);
        }

        public async Task ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public object GetById(int propertyId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetPropertiesByOwner(string ownerId)
        {
            throw new NotImplementedException();
        }
        //public IEnumerable<Property> GetPropertiesByOwner(string ownerId)
        //{
        //    return _context.Properties
        //        .Include(p => p.Registration) // Include related Registration data
        //        .Where(p => p.Owner_Id == ownerId)
        //        .ToList();
        //}

        //IEnumerable<object> IProperty.GetPropertiesByOwner(string ownerId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
