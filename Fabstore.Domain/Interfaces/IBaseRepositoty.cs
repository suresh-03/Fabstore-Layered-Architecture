namespace Fabstore.Domain.Interfaces
    {
    public interface IBaseRepositoty
        {
        public Task<bool> SaveDbChangesAsync();
        }
    }
