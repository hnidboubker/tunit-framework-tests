using IntEntityFrameworkCore.Persistence;

namespace IntInfrastructure.Configurations
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DefaultContext Db;

        public UnitOfWork(DefaultContext db)
        {
            Db = db;
        }


        public virtual int SaveChanges()
        {
            return Db.SaveChanges();
        }
        public virtual async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Db.SaveChangesAsync(cancellationToken);
        }
    }
}
