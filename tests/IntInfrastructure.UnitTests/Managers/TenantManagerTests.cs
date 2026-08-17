using IntCore.Models.MultiTenancy;
using IntInfrastructure.Contracts;
using IntInfrastructure.Managers;

namespace IntInfrastructure.UnitTests.Managers
{
    public class TenantManagerTests
    {
        [Test]
        public async Task CreateAsync_Should_Return_Tenant()
        {
            // Arrange
            var tenant = new Tenant();

            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act
            var result = await manager.CreateAsync(tenant);

            // Assert
            await Assert.That(result).IsSameReferenceAs(tenant);
            await Assert.That(repository.CreatedTenant).IsSameReferenceAs(tenant);
        }

        [Test]
        public async Task CreateAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act & Assert
            await Assert.That(async () =>
                await manager.CreateAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task EditAsync_Should_Return_Tenant()
        {
            // Arrange
            var tenant = new Tenant();

            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act
            var result = await manager.EditAsync(tenant);

            // Assert
            await Assert.That(result).IsSameReferenceAs(tenant);
            await Assert.That(repository.EditedTenant).IsSameReferenceAs(tenant);
        }

        [Test]
        public async Task EditAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act & Assert
            await Assert.That(async () =>
                await manager.EditAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task RemoveAsync_Should_Remove_Tenant()
        {
            // Arrange
            var tenant = new Tenant();

            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act
            await manager.RemoveAsync(tenant);

            // Assert
            await Assert.That(repository.RemovedTenant).IsSameReferenceAs(tenant);
        }

        [Test]
        public async Task RemoveAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act & Assert
            await Assert.That(async () =>
                await manager.RemoveAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task DeleteAsync_Should_Set_IsDeleted_And_Edit_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                IsDeleted = false
            };

            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act
            await manager.DeleteAsync(tenant);

            // Assert
            await Assert.That(tenant.IsDeleted).IsTrue();
            await Assert.That(repository.EditedTenant).IsSameReferenceAs(tenant);
        }

        [Test]
        public async Task DeleteAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act & Assert
            await Assert.That(async () =>
                await manager.DeleteAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task Tenants_Should_Return_Repository_Tenants()
        {
            // Arrange
            var repository = new FakeTenantRepository();
            var manager = new TenantManager(repository);

            // Act
            var result = manager.Tenants;

            // Assert - check sequence equality since IQueryable is recreated on each access
            await Assert.That(result).IsEquivalentTo(repository.Tenants);
        }


        private class FakeTenantRepository : ITenantRepository
        {
            private readonly List<Tenant> _tenants = new();

            public Tenant? CreatedTenant { get; private set; }
            public Tenant? EditedTenant { get; private set; }
            public Tenant? RemovedTenant { get; private set; }

            public IQueryable<Tenant> Tenants => _tenants.AsQueryable();

            public Task<Tenant> CreateAsync(Tenant tenant)
            {
                CreatedTenant = tenant;
                _tenants.Add(tenant);

                return Task.FromResult(tenant);
            }

            public Task<Tenant> EditAsync(Tenant tenant)
            {
                EditedTenant = tenant;

                return Task.FromResult(tenant);
            }

            public Task RemoveAsync(Tenant tenant)
            {
                RemovedTenant = tenant;

                return Task.CompletedTask;
            }
        }
    }
}
