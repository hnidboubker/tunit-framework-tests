using IntCore.Models.MultiTenancy;
using IntInfrastructure.Contracts;

namespace IntInfrastructure.Managers
{
    public interface ITenantManager
    {
        IQueryable<Tenant> Tenants { get; }

        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant> EditAsync(Tenant tenant);
        Task RemoveAsync(Tenant tenant);
        Task DeleteAsync(Tenant tenant);
    }

    public class TenantManager : ITenantManager
    {
        private readonly ITenantRepository TenantRepository;

        public TenantManager(ITenantRepository tenantRepository)
        {
            TenantRepository = tenantRepository;
        }

        public virtual IQueryable<Tenant> Tenants
        {
            get
            {
                return TenantRepository.Tenants;
            }
        }

        public virtual async Task<Tenant> CreateAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return await TenantRepository.CreateAsync(tenant);
        }

        public virtual async Task<Tenant> EditAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return await TenantRepository.EditAsync(tenant);
        }

        public virtual async Task RemoveAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            await TenantRepository.RemoveAsync(tenant);
        }

        public virtual async Task DeleteAsync(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            tenant.IsDeleted = true;

            await TenantRepository.EditAsync(tenant);
        }
    }
}
