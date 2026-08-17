using IntCore.Models.MultiTenancy;
using IntEntityFrameworkCore.Persistence;
using IntInfrastructure.Configurations;
using Microsoft.EntityFrameworkCore;


namespace IntInfrastructure.UnitTests.Configurations
{
    public class UnitOfWorkTests
    {
        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DefaultContext(options);
        }

        [Test]
        public async Task SaveChanges_Should_Save_Changes()
        {
            // Arrange
            using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            context.Set<Tenant>().Add(new Tenant { Name = "Test Tenant" });

            // Act
            var result = unitOfWork.SaveChanges();

            // Assert
            await Assert.That(result).IsEqualTo(1);
            await Assert.That(context.Set<Tenant>().Count()).IsEqualTo(1);
        }

        [Test]
        public async Task SaveChangesAsync_Should_Save_Changes()
        {
            // Arrange
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            context.Set<Tenant>().Add(new Tenant { Name = "Test Tenant" });

            // Act
            var result = await unitOfWork.SaveChangesAsync();

            // Assert
            await Assert.That(result).IsEqualTo(1);
        }

        [Test]
        public async Task SaveChangesAsync_WithCancellationToken_Should_Save_Changes()
        {
            // Arrange
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            context.Set<Tenant>().Add(new Tenant { Name = "Test Tenant" });

            using var cts = new CancellationTokenSource();

            // Act
            var result = await unitOfWork.SaveChangesAsync(cts.Token);

            // Assert
            await Assert.That(result).IsEqualTo(1);
        }
    }
}

