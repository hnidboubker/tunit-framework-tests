using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;

namespace IntCore.UnitTests
{
    public class TenantTests
    {
        [Test]
        public async Task Should_Create_Tenant()
        {
            // Arrange & Act
            var tenant = new Tenant();

            // Assert
            await Assert.That(tenant).IsNotNull();
        }

        [Test]
        public async Task Should_Have_Default_Id()
        {
            // Arrange
            var tenant = new Tenant();

            // Assert
            await Assert.That(tenant.Id).IsEqualTo(0);
        }

        [Test]
        public async Task Should_Set_Id()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 10
            };

            // Assert
            await Assert.That(tenant.Id).IsEqualTo(10);
        }

        [Test]
        public async Task Should_Set_Name()
        {
            // Arrange
            var tenant = new Tenant
            {
                Name = "Acme"
            };

            // Assert
            await Assert.That(tenant.Name).IsEqualTo("Acme");
        }

        [Test]
        public async Task Should_Initialize_Users_Collection()
        {
            // Arrange
            var tenant = new Tenant();

            // Assert
            await Assert.That(tenant.Users).IsNotNull();
        }

        [Test]
        public async Task Users_Should_Be_Empty_By_Default()
        {
            // Arrange
            var tenant = new Tenant();

            // Assert
            await Assert.That(tenant.Users).IsEmpty();
        }

        [Test]
        public async Task Should_Add_User_To_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Acme"
            };

            var user = new User
            {
                Id = 100,
                FirstName = "John",
                LastName = "Doe",
                TenantId = tenant.Id
            };

            // Act
            tenant.Users.Add(user);

            // Assert
            await Assert.That(tenant.Users).Count().IsEqualTo(1);
            await Assert.That(tenant.Users).Contains(user);
        }

        [Test]
        public async Task Should_Add_Multiple_Users_To_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Acme"
            };

            var user1 = new User
            {
                Id = 100,
                FirstName = "John",
                LastName = "Doe",
                TenantId = tenant.Id
            };

            var user2 = new User
            {
                Id = 101,
                FirstName = "Jane",
                LastName = "Doe",
                TenantId = tenant.Id
            };

            // Act
            tenant.Users.Add(user1);
            tenant.Users.Add(user2);

            // Assert
            await Assert.That(tenant.Users).Count().IsEqualTo(2);
            await Assert.That(tenant.Users).Contains(user1);
            await Assert.That(tenant.Users).Contains(user2);
        }

        [Test]
        public async Task Should_Remove_User_From_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Acme"
            };

            var user = new User
            {
                Id = 100,
                FirstName = "John",
                LastName = "Doe",
                TenantId = tenant.Id
            };

            tenant.Users.Add(user);

            // Act
            var removed = tenant.Users.Remove(user);

            // Assert
            await Assert.That(removed).IsTrue();
            await Assert.That(tenant.Users).IsEmpty();
        }

        [Test]
        public async Task Should_Update_Tenant_Name()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Old Name"
            };

            // Act
            tenant.Name = "New Name";

            // Assert
            await Assert.That(tenant.Name).IsEqualTo("New Name");
        }

        [Test]
        public async Task Should_Create_Tenant_With_All_Properties()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 10,
                Name = "My Company"
            };

            var user = new User
            {
                Id = 50,
                FirstName = "Alice",
                LastName = "Smith",
                TenantId = tenant.Id
            };

            tenant.Users.Add(user);

            // Assert
            await Assert.That(tenant.Id).IsEqualTo(10);
            await Assert.That(tenant.Name).IsEqualTo("My Company");
            await Assert.That(tenant.Users).Count().IsEqualTo(1);
            await Assert.That(tenant.Users).Contains(user);
        }
    }
}
