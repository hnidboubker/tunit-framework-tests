using IntCore.Models.MultiTenancy;
using IntEntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IntInfrastructure.Contracts
{
    public interface ITenantRepository
    {
        IQueryable<Tenant> Tenants { get; }
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant> EditAsync(Tenant tenant);
        Task RemoveAsync(Tenant tenant);
    }

    public class TenantRepository : ITenantRepository
    {
        private readonly DefaultContext Db;
        private readonly DbSet<Tenant> DbSet;

        public TenantRepository(DefaultContext db)
        {
            Db = db;
            DbSet = Db.Set<Tenant>();
        }

        public virtual IQueryable<Tenant> Tenants
        {
            get
            {
                return DbSet.AsQueryable();
            }
        }

        public virtual async Task<Tenant> CreateAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            await DbSet.AddAsync(tenant);

            return tenant;
        }

        public virtual Task<Tenant> EditAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            DbSet.Update(tenant);

            return Task.FromResult(tenant);
        }

        public virtual Task RemoveAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            DbSet.Remove(tenant);

            return Task.CompletedTask;
        }
    }
}
