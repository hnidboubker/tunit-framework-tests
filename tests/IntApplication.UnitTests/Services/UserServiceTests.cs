using IntApplication.DTOs;
using IntApplication.Services;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace IntApplication.UnitTests.Services
{



    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userManagerMock = CreateUserManagerMock();
            _sut = new UserServiceBui UserService(_userManagerMock.Object);
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

        // ============================================================
        // GetUsersAsync
        // ============================================================

        [Test]
        public async Task GetUsersAsync_Should_ReturnEmptyList_WhenNoUsersExist()
        {



            // Arrange
            var users = new List<User>();

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(users));

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsEmpty();
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnAllUsers()
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

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(users));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await _sut.GetUsersAsync();

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
        public async Task GetUsersAsync_ShouldMapUserProperties()
        {
            // Arrange
            var user = new User
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

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new[] { "Admin" });

            // Act
            var result = await _sut.GetUsersAsync();

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
        public async Task GetUsersAsync_ShouldReturnEmptyTenant_WhenTenantIsNull()
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

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(Array.Empty<string>());

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Tenant)
                .IsEqualTo("");
        }

        [Test]
        public async Task GetUsersAsync_ShouldGetRolesForEachUser()
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

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[]
                {
                admin,
                normalUser
                }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(admin))
                .ReturnsAsync(new[] { "Admin" });

            _userManagerMock
                .Setup(x => x.GetRolesAsync(normalUser))
                .ReturnsAsync(new[] { "User" });

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Roles)
                .Contains("Admin");

            await Assert.That(result[1].Roles)
                .Contains("User");

            _userManagerMock.Verify(
                x => x.GetRolesAsync(admin),
                Times.Once);

            _userManagerMock.Verify(
                x => x.GetRolesAsync(normalUser),
                Times.Once);
        }

        // ============================================================
        // CreateAsync
        // ============================================================

        [Test]
        public async Task CreateAsync_ShouldCreateUserWithCorrectProperties()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .Callback<User, string>((user, _) =>
                {
                    capturedUser = user;
                })
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.CreateAsync(dto);

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
        public async Task CreateAsync_ShouldPassPasswordToUserManager()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<User>(),
                    "Password123!"),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldAddRoles_WhenRolesAreProvided()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.Is<IEnumerable<string>>(roles =>
                        roles.SequenceEqual(
                            new[] { "Admin", "Manager" }))),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldNotAddRoles_WhenRolesAreNull()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldNotAddRoles_WhenRolesAreEmpty()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldThrowException_WhenCreationFails()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(identityResult);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _sut.CreateAsync(dto));

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Email already exists, Email is invalid");
        }

        [Test]
        public async Task CreateAsync_ShouldNotAddRoles_WhenCreationFails()
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

            _userManagerMock
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
                () => _sut.CreateAsync(dto));

            // Assert
            _userManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldAllowNullTenantId()
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

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.CreateAsync(
                    It.Is<User>(u =>
                        u.TenantId == null),
                    dto.Password),
                Times.Once);
        }

        // ============================================================
        // IQueryable helper
        // ============================================================

        private static IQueryable<User> CreateAsyncQueryable(
            IEnumerable<User> users)
        {
            return new TestAsyncEnumerable<User>(users);
        }
    }
}
