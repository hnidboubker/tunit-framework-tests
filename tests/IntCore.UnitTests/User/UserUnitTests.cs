
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using Assert = TUnit.Assertions.Assert;

namespace IntCore.UnitTests
{


    public class UserUnitTests
    {
        [Test]
        public async Task Should_Create_User_With_Default_Identity_Values()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            await Assert.That(user).IsNotNull();
            await Assert.That(user.Id).IsEqualTo(0);
            await Assert.That(user.UserName).IsNull();
            await Assert.That(user.Email).IsNull();
        }

        [Test]
        public async Task Should_Set_User_Personal_Information()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Status = "Active",
                Avatar = "avatar.png"
            };

            // Assert
            await Assert.That(user.FirstName).IsEqualTo("John");
            await Assert.That(user.LastName).IsEqualTo("Doe");
            await Assert.That(user.Status).IsEqualTo("Active");
            await Assert.That(user.Avatar).IsEqualTo("avatar.png");
        }

        [Test]
        public async Task Should_Set_Identity_Properties()
        {
            // Arrange
            var user = new User
            {
                Id = 42,
                UserName = "john.doe",
                Email = "john.doe@test.com",
                PhoneNumber = "+33123456789"
            };

            // Assert
            await Assert.That(user.Id).IsEqualTo(42);
            await Assert.That(user.UserName).IsEqualTo("john.doe");
            await Assert.That(user.Email).IsEqualTo("john.doe@test.com");
            await Assert.That(user.PhoneNumber).IsEqualTo("+33123456789");
        }

        [Test]
        public async Task Should_Set_Tenant_Id()
        {
            // Arrange
            var user = new User
            {
                TenantId = 10
            };

            // Assert
            await Assert.That(user.TenantId).IsEqualTo(10);
        }

        [Test]
        public async Task Tenant_Id_Should_Be_Null_By_Default()
        {
            // Arrange
            var user = new User();

            // Assert
            await Assert.That(user.TenantId).IsNull();
        }

        [Test]
        public async Task Should_Associate_User_With_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 10
            };

            var user = new User
            {
                TenantId = tenant.Id,
                Tenant = tenant
            };

            // Assert
            await Assert.That(user.Tenant).IsNotNull();
            await Assert.That(user.Tenant).IsSameReferenceAs(tenant);
            await Assert.That(user.TenantId).IsEqualTo(tenant.Id);
        }

        [Test]
        public async Task Tenant_Should_Be_Null_By_Default()
        {
            // Arrange
            var user = new User();

            // Assert
            await Assert.That(user.Tenant).IsNull();
        }

        [Test]
        public async Task Should_Allow_Updating_User_Properties()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Status = "Active",
                Avatar = "old.png"
            };

            // Act
            user.FirstName = "Jane";
            user.LastName = "Smith";
            user.Status = "Inactive";
            user.Avatar = "new.png";

            // Assert
            await Assert.That(user.FirstName).IsEqualTo("Jane");
            await Assert.That(user.LastName).IsEqualTo("Smith");
            await Assert.That(user.Status).IsEqualTo("Inactive");
            await Assert.That(user.Avatar).IsEqualTo("new.png");
        }

        [Test]
        public async Task Should_Allow_Changing_Tenant()
        {
            // Arrange
            var firstTenant = new Tenant { Id = 1 };
            var secondTenant = new Tenant { Id = 2 };

            var user = new User
            {
                TenantId = firstTenant.Id,
                Tenant = firstTenant
            };

            // Act
            user.TenantId = secondTenant.Id;
            user.Tenant = secondTenant;

            // Assert
            await Assert.That(user.TenantId).IsEqualTo(2);
            await Assert.That(user.Tenant).IsSameReferenceAs(secondTenant);
        }

        [Test]
        public async Task Should_Allow_Removing_Tenant()
        {
            // Arrange
            var tenant = new Tenant { Id = 1 };

            var user = new User
            {
                TenantId = tenant.Id,
                Tenant = tenant
            };

            // Act
            user.TenantId = null;
            user.Tenant = null;

            // Assert
            await Assert.That(user.TenantId).IsNull();
            await Assert.That(user.Tenant).IsNull();
        }
    }
}
