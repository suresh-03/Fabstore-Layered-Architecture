namespace Fabstore.DataAccess
    {
    public interface IBaseRepository
        {
        public Task<bool> SaveDbChangesAsync();
        }
    }
