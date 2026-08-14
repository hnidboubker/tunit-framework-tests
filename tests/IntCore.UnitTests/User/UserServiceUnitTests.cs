using IntCore.DTOs;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using IntCore.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Assert = TUnit.Assertions.Assert;

namespace IntCore.UnitTests
{
    public class UserServiceUnitTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly UserService _sut;

        public UserServiceUnitTests()
        {
            _userManagerMock = CreateUserManagerMock();
            _sut = new UserService(_userManagerMock.Object);
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                null!, // IOptions<IdentityOptions>
                null!, // IPasswordHasher<User>
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                null!, // ILookupNormalizer
                new IdentityErrorDescriber(),
                null!, // IServiceProvider
                null!  // ILogger<UserManager<User>>
            );
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnEmptyList_WhenThereAreNoUsers()
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
        public async Task GetUsersAsync_ShouldMapUserProperties()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                Tenant = new Tenant
                {
                    Name = "Tenant A"
                }
            };

            var users = new List<User> { user };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(users));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result).Count().IsEqualTo(1);

            var dto = result[0];

            await Assert.That(dto.Id).IsEqualTo(1);
            await Assert.That(dto.FirstName).IsEqualTo("John");
            await Assert.That(dto.LastName).IsEqualTo("Doe");
            await Assert.That(dto.FullName).IsEqualTo("John Doe");
            await Assert.That(dto.Email).IsEqualTo("john.doe@test.com");
            await Assert.That(dto.Tenant).IsEqualTo("Tenant A");
            await Assert.That(dto.Roles).Contains("Admin");
        }

        [Test]
        public async Task GetUsersAsync_ShouldUseEmptyTenant_WhenTenantIsNull()
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
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Tenant).IsEqualTo("");
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
                        UserName = "test1"
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Jane",
                        LastName = "Doe",
                        Email = "jane@test.com",
                        UserName = "test2"
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Bob",
                        LastName = "Smith",
                        Email = "bob@test.com",
                        UserName = "test3"
                    }
                };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(users));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string>());

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
        public async Task GetUsersAsync_ShouldReturnEmptyRoles_WhenUserHasNoRoles()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Roles).IsNotNull();
            await Assert.That(result[0].Roles).IsEmpty();
        }

        [Test]
        public async Task GetUsersAsync_ShouldPreserveMultipleRoles()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string>
                    {
                    "Admin",
                    "Manager",
                    "User"
                    });

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Roles).Count().IsEqualTo(3);
            await Assert.That(result[0].Roles).Contains("Admin");
            await Assert.That(result[0].Roles).Contains("Manager");
            await Assert.That(result[0].Roles).Contains("User");
        }

        [Test]
        public async Task GetUsersAsync_ShouldBuildFullNameCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Jean",
                LastName = "Dupont",
                Email = "jean@test.com"
            };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].FullName)
                .IsEqualTo("Jean Dupont");
        }


        [Test]
        public async Task GetUsersAsync_ShouldNotModifySourceUsers()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            var users = new List<User> { user };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(users));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            // Act
            await _sut.GetUsersAsync();

            // Assert
            await Assert.That(user.Id).IsEqualTo(1);
            await Assert.That(user.FirstName).IsEqualTo("John");
            await Assert.That(user.LastName).IsEqualTo("Doe");
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

            var user = new User
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "User",
                Email = "user@test.com"
            };

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(CreateAsyncQueryable(new[] { admin, user }));

            _userManagerMock
                .Setup(x => x.GetRolesAsync(admin))
                .ReturnsAsync(new List<string> { "Admin" });

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _sut.GetUsersAsync();

            // Assert
            await Assert.That(result[0].Roles).Contains("Admin");
            await Assert.That(result[1].Roles).Contains("User");

            _userManagerMock.Verify(
                x => x.GetRolesAsync(admin),
                Times.Once);

            _userManagerMock.Verify(
                x => x.GetRolesAsync(user),
                Times.Once);
        }


        [Test]
        public async Task CreateAsync_ShouldCreateUserWithCorrectProperties()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "jhontest",
                Email = "john@test.com",
                Password = "123Qwe!",
                TenantId = 10
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            await Assert.That(result).IsNotNull();

            _userManagerMock.Verify(
                x => x.CreateAsync(
                    It.Is<User>(u =>
                        u.FirstName == "John" &&
                        u.LastName == "Doe" &&
                        u.UserName == "jhontest" &&
                        u.Email == "john@test.com" &&
                        u.TenantId == 10),
                    "123Qwe!"),
                Times.Once);
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
                Password = "MyPassword123!",
                TenantId = 10
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), "MyPassword123!"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateAsync(dto);

            // Assert
            _userManagerMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<User>(),
                    "MyPassword123!"),
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
                UserName = "test1",
                Password = "123Qwe",
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
                        roles.Contains("Admin") &&
                        roles.Contains("Manager"))),
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
                Password = "Password123!",
                TenantId = 10,
                Roles = null
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
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
                Password = "Password123!",
                TenantId = 10,
                Roles = Array.Empty<string>()
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
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
        public async Task CreateAsync_ShouldThrowException_WhenUserCreationFails()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Password123!",
                TenantId = 10
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
                .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
                .ReturnsAsync(identityResult);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _sut.CreateAsync(dto));

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Email already exists, Email is invalid");

            _userManagerMock.Verify(
                x => x.AddToRolesAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task CreateAsync_ShouldNotAddRoles_WhenUserCreationFails()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Password123!",
                TenantId = 10,
                Roles = new[] { "Admin" }
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
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
                UserName = "test1",
                Password = "123Qws!",
                TenantId = null
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            await Assert.That(result.Succeeded).IsTrue();

            _userManagerMock.Verify(
                x => x.CreateAsync(
                    It.Is<User>(u =>
                        u.FirstName == "John" &&
                        u.LastName == "Doe" &&
                        u.Email == "john@test.com" &&
                        u.UserName == "test1" &&
                        u.TenantId == null),
                    dto.Password),
                Times.Once);
        }
        private static IQueryable<User> CreateAsyncQueryable(
            IEnumerable<User> users)
        {
            var queryable = users.AsQueryable();

            return new TestAsyncEnumerable<User>(queryable);
        }
    }
}
