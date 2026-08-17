using IntCore.Models.MultiTenancy;
using IntEntityFrameworkCore.Persistence;
using IntInfrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IntInfrastructure.UnitTests.Contracts
{
    public class TenantRepositoryTests
    {
        private static DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DefaultContext(options);
        }

        [Test]
        public async Task CreateAsync_Should_Add_Tenant()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            var tenant = new Tenant
            {
                // Id = ...
                // Name = "Test"
            };

            // Act
            var result = await repository.CreateAsync(tenant);

            // Assert
            await Assert.That(result).IsSameReferenceAs(tenant);
            await Assert.That(context.Set<Tenant>().Local).Contains(tenant);
        }

        [Test]
        public async Task CreateAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            // Act & Assert
            await Assert.That(async () =>
                await repository.CreateAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task EditAsync_Should_Update_Tenant()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            var tenant = new Tenant
            {
                // Id = ...
                // Name = "Test"
            };

            await context.Set<Tenant>().AddAsync(tenant);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.EditAsync(tenant);

            // Assert
            await Assert.That(result).IsSameReferenceAs(tenant);
            await Assert.That(context.Entry(tenant).State)
                 .IsEqualTo(EntityState.Modified);
        }

        [Test]
        public async Task EditAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            // Act & Assert
            await Assert.That(async () =>
                await repository.EditAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task RemoveAsync_Should_Remove_Tenant()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            var tenant = new Tenant
            {
                // Id = ...
                // Name = "Test"
            };

            await context.Set<Tenant>().AddAsync(tenant);
            await context.SaveChangesAsync();

            // Act
            await repository.RemoveAsync(tenant);

            // Assert
            await Assert.That(context.Entry(tenant).State)
                 .IsEqualTo(EntityState.Deleted);
        }

        [Test]
        public async Task RemoveAsync_Should_Throw_When_Tenant_Is_Null()
        {
            // Arrange
            await using var context = CreateContext();
            var repository = new TenantRepository(context);

            // Act & Assert
            await Assert.That(async () =>
                await repository.RemoveAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task Tenants_Should_Return_Tenants()
        {
            // Arrange
            await using var context = CreateContext();

            var tenant1 = new Tenant
            {
                // Id = ...
                // Name = "Tenant 1"
            };

            var tenant2 = new Tenant
            {
                // Id = ...
                // Name = "Tenant 2"
            };

            await context.Set<Tenant>().AddRangeAsync(tenant1, tenant2);
            await context.SaveChangesAsync();

            var repository = new TenantRepository(context);

            // Act
            var tenants = repository.Tenants.ToList();

            // Assert
            await Assert.That(tenants).HasCount(2);
        }
    }
}
