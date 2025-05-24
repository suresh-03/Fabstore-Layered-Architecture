namespace Fabstore.Domain.Interfaces
    {
    public interface IBaseRepository
        {
        public Task<bool> SaveDbChangesAsync();
        }
    }
