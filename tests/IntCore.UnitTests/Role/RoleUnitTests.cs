using IntCore.Models.Identity;
namespace IntCore.UnitTests
{


    public class RoleUnitTests
    {
        [Test]
        public async Task Should_Create_Role()
        {
            // Arrange & Act
            var role = new Role();

            // Assert
            await Assert.That(role).IsNotNull();
        }

        [Test]
        public async Task Should_Have_Default_Id()
        {
            // Arrange
            var role = new Role();

            // Assert
            await Assert.That(role.Id).IsEqualTo(0);
        }

        [Test]
        public async Task Should_Set_Id()
        {
            // Arrange
            var role = new Role
            {
                Id = 42
            };

            // Assert
            await Assert.That(role.Id).IsEqualTo(42);
        }

        [Test]
        public async Task Should_Set_Name()
        {
            // Arrange
            var role = new Role
            {
                Name = "Administrator"
            };

            // Assert
            await Assert.That(role.Name).IsEqualTo("Administrator");
        }

        [Test]
        public async Task Should_Set_Normalized_Name()
        {
            // Arrange
            var role = new Role
            {
                NormalizedName = "ADMINISTRATOR"
            };

            // Assert
            await Assert.That(role.NormalizedName).IsEqualTo("ADMINISTRATOR");
        }

        [Test]
        public async Task Should_Set_Name_And_Normalized_Name()
        {
            // Arrange
            var role = new Role
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            };

            // Assert
            await Assert.That(role.Name).IsEqualTo("Administrator");
            await Assert.That(role.NormalizedName).IsEqualTo("ADMINISTRATOR");
        }

        [Test]
        public async Task Should_Allow_Updating_Name()
        {
            // Arrange
            var role = new Role
            {
                Name = "User"
            };

            // Act
            role.Name = "Administrator";

            // Assert
            await Assert.That(role.Name).IsEqualTo("Administrator");
        }

        [Test]
        public async Task Should_Allow_Updating_Normalized_Name()
        {
            // Arrange
            var role = new Role
            {
                NormalizedName = "USER"
            };

            // Act
            role.NormalizedName = "ADMINISTRATOR";

            // Assert
            await Assert.That(role.NormalizedName)
                .IsEqualTo("ADMINISTRATOR");
        }

        [Test]
        public async Task Should_Allow_Null_Name()
        {
            // Arrange
            var role = new Role();

            // Act
            role.Name = null;

            // Assert
            await Assert.That(role.Name).IsNull();
        }

        [Test]
        public async Task Should_Allow_Null_Normalized_Name()
        {
            // Arrange
            var role = new Role();

            // Act
            role.NormalizedName = null;

            // Assert
            await Assert.That(role.NormalizedName).IsNull();
        }

        [Test]
        public async Task Should_Create_Role_With_All_Properties()
        {
            // Arrange
            var role = new Role
            {
                Id = 10,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            };

            // Assert
            await Assert.That(role.Id).IsEqualTo(10);
            await Assert.That(role.Name).IsEqualTo("Administrator");
            await Assert.That(role.NormalizedName)
                .IsEqualTo("ADMINISTRATOR");
        }
    }
}
