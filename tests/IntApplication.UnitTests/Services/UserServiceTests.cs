using IntApplication.DTOs;
using IntApplication.Services;
using IntApplication.UnitTests.Helpers;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace IntApplication.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> UserManagerMock;
        private readonly Mock<RoleManager<Role>> RoleManagerMock; 
        private readonly  IUserService Sut;

        public UserServiceTests()
        {
            UserManagerMock = CreateUserManagerMock();
            RoleManagerMock  = CreateRoleManagerMock();
            Sut = new UserService(UserManagerMock.Object, RoleManagerMock.Object);
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                null!,
                new IdentityErrorDescriber(),
                null!,
                null!);
        }

        private static Mock<RoleManager<Role>> CreateRoleManagerMock()
        {
            var store = new Mock<IRoleStore<Role>>();

            return new Mock<RoleManager<Role>>(
                store.Object,
                Array.Empty<IRoleValidator<Role>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!);
        }

        // ============================================================
        // GetUsersAsync
        // ============================================================

        [Test]
        public async Task GetUsersAsync_Should_Return_EmptyList_When_NoUsers_Exist()
        {



            // Arrange
            var users = new List<User>();
            var helper = QuerableHelper.CreateUserAsyncQueryable(users);
            UserManagerMock
                .Setup(x => x.Users)
                .Returns(helper);

            // Act
            var result = await Sut.GetUsersAsync();

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsEmpty();
        }

        [Test]
        public async Task GetUsersAsync_Should_Return_AllUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john"
            },
            new()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@test.com",
                UserName = "jane"
            },
            new()
            {
                Id = 3,
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob@test.com",
                UserName = "bob"
            }
        };

            var helper = QuerableHelper.CreateUserAsyncQueryable(users);
            UserManagerMock
                .Setup(x => x.Users)
                .Returns(helper);

            UserManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await Sut.GetUsersAsync();

            // Assert
            await Assert.That(result).Count().IsEqualTo(3);

            await Assert.That(result.Select(x => x.Id))
                .Contains(1);

            await Assert.That(result.Select(x => x.Id))
                .Contains(2);

            await Assert.That(result.Select(x => x.Id))
                .Contains(3);
        }

        [Test]
        public async Task GetUsersAsync_Should_MapUser_Properties()
        {
            // Arrange
            var user = new IntCore.Models.Identity.User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Tenant = new Tenant
                {
                    Id = 10,
                    Name = "My Tenant"
                }
            };
            var helper = QuerableHelper.CreateUserAsyncQueryable(new[] { user });
            UserManagerMock
                .Setup(x => x.Users)
                .Returns(helper);

            UserManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new[] { "Admin" });

            // Act
            var result = await Sut.GetUsersAsync();

            // Assert
            await Assert.That(result).Count().IsEqualTo(1);

            var dto = result[0];

            await Assert.That(dto.Id).IsEqualTo(1);
            await Assert.That(dto.FirstName).IsEqualTo("John");
            await Assert.That(dto.LastName).IsEqualTo("Doe");
            await Assert.That(dto.FullName).IsEqualTo("John Doe");
            await Assert.That(dto.Email).IsEqualTo("john@test.com");
            await Assert.That(dto.Tenant).IsEqualTo("My Tenant");

            await Assert.That(dto.Roles)
                .Contains("Admin");
        }

        [Test]
        public async Task GetUsersAsync_Should_Return_EmptyTenant_When_Tenant_IsNull()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Tenant = null
            };
            var helper = QuerableHelper.CreateUserAsyncQueryable(new[] { user });
            UserManagerMock
                .Setup(x => x.Users)
                .Returns(helper);

            UserManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await Sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Tenant)
                .IsEqualTo("");
        }

        [Test]
        public async Task GetUsersAsync_Should_GetRoles_ForEach_User()
        {
            // Arrange
            var admin = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Admin",
                Email = "admin@test.com"
            };

            var normalUser = new User
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "User",
                Email = "user@test.com"
            };
            var helper = QuerableHelper.CreateUserAsyncQueryable(new[]
                {
                admin,
                normalUser
                });

            UserManagerMock
                .Setup(x => x.Users)
                .Returns(helper);

            UserManagerMock
                .Setup(x => x.GetRolesAsync(admin))
                .ReturnsAsync(new[] { "Admin" });

            UserManagerMock
                .Setup(x => x.GetRolesAsync(normalUser))
                .ReturnsAsync(new[] { "User" });

            // Act
            var result = await Sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Roles)
                .Contains("Admin");

            await Assert.That(result[1].Roles)
                .Contains("User");

            UserManagerMock.Verify(
                x => x.GetRolesAsync(admin),
                Times.Once);

            UserManagerMock.Verify(
                x => x.GetRolesAsync(normalUser),
                Times.Once);
        }

        // ============================================================
        // CreateAsync
        // ============================================================

        [Test]
        public async Task CreateAsync_Should_Create_User_With_Correct_Properties()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                TenantId = 10
            };

            User? capturedUser = null;

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .Callback<User, string>((user, _) =>
                {
                    capturedUser = user;
                })
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await Sut.CreateAsync(dto);

            // Assert
            await Assert.That(result.Succeeded).IsTrue();
            await Assert.That(capturedUser).IsNotNull();

            await Assert.That(capturedUser!.FirstName)
                .IsEqualTo("John");

            await Assert.That(capturedUser.LastName)
                .IsEqualTo("Doe");

            await Assert.That(capturedUser.Email)
                .IsEqualTo("john@test.com");

            await Assert.That(capturedUser.UserName)
                .IsEqualTo("john");

            await Assert.That(capturedUser.TenantId)
                .IsEqualTo(10);
        }

        [Test]
        public async Task CreateAsync_Should_Pass_Password_To_UserManager()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!"
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await Sut.CreateAsync(dto);

            // Assert
            UserManagerMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<User>(),
                    "Password123!"),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_Should_AddRoles_When_Roles_Are_Provided()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                TenantId = 10,
                Roles = new[] { "Admin", "Manager" }
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            UserManagerMock
                .Setup(x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await Sut.CreateAsync(dto);

            // Assert
            UserManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.Is<IEnumerable<string>>(roles =>
                        roles.SequenceEqual(
                            new[] { "Admin", "Manager" }))),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldNot_AddRoles_When_Roles_Are_Null()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                Roles = null
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await Sut.CreateAsync(dto);

            // Assert
            UserManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldNot_Add_Roles_When_Roles_Are_Empty()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                Roles = Array.Empty<string>()
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await Sut.CreateAsync(dto);

            // Assert
            UserManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldThrow_Exception_When_Creation_Fails()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!"
            };

            var identityResult = IdentityResult.Failed(
                new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email already exists"
                },
                new IdentityError
                {
                    Code = "InvalidEmail",
                    Description = "Email is invalid"
                });

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(identityResult);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Sut.CreateAsync(dto));

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Email already exists, Email is invalid");
        }

        [Test]
        public async Task CreateAsync_ShouldNot_Add_Roles_When_Creation_Fails()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                Roles = new[] { "Admin" }
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Creation failed"
                        }));

            // Act
            await Assert.ThrowsAsync<Exception>(
                () => Sut.CreateAsync(dto));

            // Assert
            UserManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldAllow_Null_TenantId()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                UserName = "john",
                Password = "Password123!",
                TenantId = null
            };

            UserManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await Sut.CreateAsync(dto);

            // Assert
            UserManagerMock.Verify(
                x => x.CreateAsync(
                    It.Is<User>(u =>
                        u.TenantId == null),
                    dto.Password),
                Times.Once);
        }
    }
}
