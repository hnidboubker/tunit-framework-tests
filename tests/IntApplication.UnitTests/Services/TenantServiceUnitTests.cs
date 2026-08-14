using IntApplication.DTOs;
using IntApplication.Services;
using IntApplication.UnitTests.Helpers;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using IntInfrastructure.Configurations;
using IntInfrastructure.Managers;
using Microsoft.AspNetCore.Identity;
using Moq;
using TUnit.Core;


namespace IntApplication.UnitTests.Services
{
    public class TenantServiceUnitTests
    {
        private const string TenantAdminRole = "TenantAdmin";

        private Mock<ITenantManager> TenantManager = null!;
        private Mock<UserManager<User>> _userManager = null!;
        private Mock<RoleManager<Role>> _roleManager = null!;
        private Mock<IUnitOfWork> _unitOfWork = null!;

        private TenantService _sut = null!;

        [Before(Test)]
        public void Setup()
        {
            TenantManager = new Mock<ITenantManager>();

            _userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _roleManager = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null,
                null,
                null,
                null);

            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _sut = new TenantService(
                TenantManager.Object,
                _userManager.Object,
                _roleManager.Object,
                _unitOfWork.Object);
        }

        // ============================================================
        // CreateAsync
        // ============================================================

        [Test]
        public async Task CreateAsync_WhenDtoIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.That(
                    () => _sut.CreateAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task CreateAsync_ShouldCreateTenant()
        {
            // Arrange
            var dto = new CreateTenantDto
            {
                Name = "Acme"
            };

           
            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Id).IsEqualTo(1);
            await Assert.That(result.Name).IsEqualTo("Acme");

            TenantManager.Verify(
                x => x.CreateAsync(
                    It.Is<Tenant>(t =>
                        t.Name == "Acme")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // ============================================================
        // CreateTenantWithUserAdminAsync
        // ============================================================

        [Test]
        public async Task CreateTenantWithUserAdminAsync_WhenDtoIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.That(
                    () => _sut.CreateTenantWithUserAdminAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task CreateTenantWithUserAdminAsync_ShouldCreateTenantUserAndRole()
        {
            // Arrange
            var dto = new CreateTenantWithUserAdminDto
            {
                TenantName = "Acme",
                Email = "admin@acme.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            _userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleManager
                .Setup(x => x.RoleExistsAsync(TenantAdminRole))
                .ReturnsAsync(false);

            _roleManager
                .Setup(x => x.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result =
                await _sut.CreateTenantWithUserAdminAsync(dto);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Id).IsEqualTo(1);
            await Assert.That(result.Name).IsEqualTo("Acme");

            TenantManager.Verify(
                x => x.CreateAsync(
                    It.Is<Tenant>(t =>
                        t.Name == "Acme")),
                Times.Once);

            _userManager.Verify(
                x => x.CreateAsync(
                    It.Is<User>(u =>
                        u.UserName == dto.Email &&
                        u.Email == dto.Email &&
                        u.FirstName == dto.FirstName &&
                        u.LastName == dto.LastName &&
                        u.TenantId == 1),
                    dto.Password),
                Times.Once);

            _roleManager.Verify(
                x => x.RoleExistsAsync(TenantAdminRole),
                Times.Once);

            _roleManager.Verify(
                x => x.CreateAsync(
                    It.Is<Role>(r =>
                        r.Name == TenantAdminRole)),
                Times.Once);

            _userManager.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Exactly(2));
        }

        [Test]
        public async Task CreateTenantWithUserAdminAsync_WhenUserCreationFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTenantWithUserAdminDto
            {
                TenantName = "Acme",
                Email = "admin@acme.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            _userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Email already exists"
                        }));

            // Act
            var exception = await Assert.That(
                    () => _sut.CreateTenantWithUserAdminAsync(dto))
                .Throws<InvalidOperationException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Email already exists");

            _roleManager.Verify(
                x => x.RoleExistsAsync(TenantAdminRole),
                Times.Never);

            _userManager.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task CreateTenantWithUserAdminAsync_WhenRoleAlreadyExists_ShouldNotCreateRole()
        {
            // Arrange
            var dto = new CreateTenantWithUserAdminDto
            {
                TenantName = "Acme",
                Email = "admin@acme.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            _userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleManager
                .Setup(x => x.RoleExistsAsync(TenantAdminRole))
                .ReturnsAsync(true);

            _userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _sut.CreateTenantWithUserAdminAsync(dto);

            // Assert
            _roleManager.Verify(
                x => x.RoleExistsAsync(TenantAdminRole),
                Times.Once);

            _roleManager.Verify(
                x => x.CreateAsync(It.IsAny<Role>()),
                Times.Never);

            _userManager.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Exactly(2));
        }

        [Test]
        public async Task CreateTenantWithUserAdminAsync_WhenRoleCreationFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTenantWithUserAdminDto
            {
                TenantName = "Acme",
                Email = "admin@acme.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            _userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleManager
                .Setup(x => x.RoleExistsAsync(TenantAdminRole))
                .ReturnsAsync(false);

            _roleManager
                .Setup(x => x.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Cannot create role"
                        }));

            // Act
            var exception = await Assert.That(
                    () => _sut.CreateTenantWithUserAdminAsync(dto))
                .Throws<InvalidOperationException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Cannot create role");

            _userManager.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task CreateTenantWithUserAdminAsync_WhenRoleAssignmentFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTenantWithUserAdminDto
            {
                TenantName = "Acme",
                Email = "admin@acme.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            TenantManager
                .Setup(x => x.CreateAsync(It.IsAny<Tenant>()))
                .Callback<Tenant>(tenant =>
                {
                    tenant.Id = 1;
                })
                .ReturnsAsync((Tenant tenant) => tenant);

            _userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleManager
                .Setup(x => x.RoleExistsAsync(TenantAdminRole))
                .ReturnsAsync(true);

            _userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    TenantAdminRole))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Role assignment failed"
                        }));

            // Act
            var exception = await Assert.That(
                    () => _sut.CreateTenantWithUserAdminAsync(dto))
                .Throws<InvalidOperationException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Role assignment failed");

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // ============================================================
        // EditAsync
        // ============================================================

        [Test]
        public async Task EditAsync_WhenDtoIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.That(
                    () => _sut.EditAsync(null!))
                .Throws<ArgumentNullException>();
        }

        [Test]
        public async Task EditAsync_WhenTenantDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        Enumerable.Empty<Tenant>()));

            var dto = new EditTenantDto
            {
                Id = 999,
                Name = "New Name"
            };

            // Act
            var exception = await Assert.That(
                    () => _sut.EditAsync(dto))
                .Throws<KeyNotFoundException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Tenant '999' not found.");

            TenantManager.Verify(
                x => x.EditAsync(It.IsAny<Tenant>()),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public async Task EditAsync_Should_Update_Tenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Old Name"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant }));

            TenantManager
                .Setup(x => x.EditAsync(It.IsAny<Tenant>()))
                .ReturnsAsync((Tenant tenant) => tenant);

            var dto = new EditTenantDto
            {
                Id = 1,
                Name = "New Name"
            };

            // Act
            var result = await _sut.EditAsync(dto);

            // Assert
            await Assert.That(result).IsSameReferenceAs(tenant);
            await Assert.That(result.Name).IsEqualTo("New Name");

            TenantManager.Verify(
                x => x.EditAsync(
                    It.Is<Tenant>(t =>
                        t.Id == 1 &&
                        t.Name == "New Name")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // ============================================================
        // EditTenantWithUserAdminAsync
        // ============================================================

        [Test]
        public async Task EditTenantWithUserAdminAsync_When_Dto_IsNull_ShouldThrow_ArgumentNull_Exception()
        {
            // Act & Assert
            await Assert.That(
                    () => _sut.EditTenantWithUserAdminAsync(null!))
                .Throws<ArgumentNullException>();
        }

        // ============================================================
        // EditTenantWithUserAdminAsync
        // ============================================================

        [Test]
        public async Task EditTenantWithUserAdminAsync_WhenTenantDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        Enumerable.Empty<Tenant>()));

            var dto = new EditTenantWithUserAdminDto
            {
                TenantId = 999,
                UserId = 1,
                TenantName = "New Tenant",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            // Act
            var exception = await Assert.That(
                    () => _sut.EditTenantWithUserAdminAsync(dto))
                .Throws<KeyNotFoundException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Tenant '999' not found.");

            TenantManager.Verify(
                x => x.EditAsync(It.IsAny<Tenant>()),
                Times.Never);

            _userManager.Verify(
                x => x.UpdateAsync(It.IsAny<User>()),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task EditTenantWithUserAdminAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Old Tenant"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant }));

            _userManager
                .Setup(x => x.Users)
                .Returns(
                    QuerableHelper.CreateUserAsyncQueryable(
                        Enumerable.Empty<User>()));

            TenantManager
                .Setup(x => x.EditAsync(It.IsAny<Tenant>()))
                .ReturnsAsync((Tenant t) => t);

            var dto = new EditTenantWithUserAdminDto
            {
                TenantId = 1,
                UserId = 999,
                TenantName = "New Tenant",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            // Act
            var exception = await Assert.That(
                    () => _sut.EditTenantWithUserAdminAsync(dto))
                .Throws<KeyNotFoundException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("User '999' not found.");

            TenantManager.Verify(
                x => x.EditAsync(
                    It.Is<Tenant>(t =>
                        t.Id == 1 &&
                        t.Name == "New Tenant")),
                Times.Once);

            _userManager.Verify(
                x => x.UpdateAsync(It.IsAny<User>()),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task EditTenantWithUserAdminAsync_WhenUserUpdateFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Old Tenant"
            };

            var user = new User
            {
                Id = 10,
                TenantId = 1,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@test.com",
                UserName = "old@test.com"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant }));

            _userManager
                .Setup(x => x.Users)
                .Returns(
                    QuerableHelper.CreateUserAsyncQueryable(
                        new[] { user }));

            TenantManager
                .Setup(x => x.EditAsync(It.IsAny<Tenant>()))
                .ReturnsAsync((Tenant t) => t);

            _userManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Invalid email"
                        }));

            var dto = new EditTenantWithUserAdminDto
            {
                TenantId = 1,
                UserId = 10,
                TenantName = "New Tenant",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            // Act
            var exception = await Assert.That(
                    () => _sut.EditTenantWithUserAdminAsync(dto))
                .Throws<InvalidOperationException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Invalid email");

            _userManager.Verify(
                x => x.UpdateAsync(
                    It.Is<User>(u =>
                        u.Id == 10 &&
                        u.TenantId == 1 &&
                        u.Email == "john@test.com" &&
                        u.UserName == "john@test.com")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task EditTenantWithUserAdminAsync_ShouldUpdateTenantAndUser()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Old Tenant"
            };

            var user = new User
            {
                Id = 10,
                TenantId = 1,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@test.com",
                UserName = "old@test.com"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant }));

            _userManager
                .Setup(x => x.Users)
                .Returns(
                    QuerableHelper.CreateUserAsyncQueryable(
                        new[] { user }));

            TenantManager
                .Setup(x => x.EditAsync(It.IsAny<Tenant>()))
                .ReturnsAsync((Tenant t) => t);

            _userManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var dto = new EditTenantWithUserAdminDto
            {
                TenantId = 1,
                UserId = 10,
                TenantName = "New Tenant",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            };

            // Act
            var result =
                await _sut.EditTenantWithUserAdminAsync(dto);

            // Assert
            await Assert.That(result)
                .IsSameReferenceAs(tenant);

            await Assert.That(tenant.Name)
                .IsEqualTo("New Tenant");

            await Assert.That(user.FirstName)
                .IsEqualTo("John");

            await Assert.That(user.LastName)
                .IsEqualTo("Doe");

            await Assert.That(user.Email)
                .IsEqualTo("john@test.com");

            await Assert.That(user.UserName)
                .IsEqualTo("john@test.com");

            TenantManager.Verify(
                x => x.EditAsync(
                    It.Is<Tenant>(t =>
                        t.Id == 1 &&
                        t.Name == "New Tenant")),
                Times.Once);

            _userManager.Verify(
                x => x.UpdateAsync(
                    It.Is<User>(u =>
                        u.Id == 10 &&
                        u.TenantId == 1 &&
                        u.FirstName == "John" &&
                        u.LastName == "Doe" &&
                        u.Email == "john@test.com" &&
                        u.UserName == "john@test.com")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        // ============================================================
        // RemoveAsync
        // ============================================================

        [Test]
        public async Task RemoveAsync_WhenTenantDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        Enumerable.Empty<Tenant>()));

            // Act
            var exception = await Assert.That(
                    () => _sut.RemoveAsync(999))
                .Throws<KeyNotFoundException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Tenant '999' not found.");

            TenantManager.Verify(
                x => x.RemoveAsync(It.IsAny<Tenant>()),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task RemoveAsync_ShouldRemoveTenantAndSave()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Acme"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant }));

            TenantManager
                .Setup(x => x.RemoveAsync(It.IsAny<Tenant>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _sut.RemoveAsync(1);

            // Assert
            TenantManager.Verify(
                x => x.RemoveAsync(
                    It.Is<Tenant>(t =>
                        t.Id == 1 &&
                        t.Name == "Acme")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        // ============================================================
        // DeleteAsync
        // ============================================================

        [Test]
        public async Task DeleteAsync_WhenTenantDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        Enumerable.Empty<Tenant>()));

            // Act
            var exception = await Assert.That(
                    () => _sut.DeleteAsync(999))
                .Throws<KeyNotFoundException>();

            // Assert
            await Assert.That(exception.Message)
                .IsEqualTo("Tenant '999' not found.");

            TenantManager.Verify(
                x => x.DeleteAsync(It.IsAny<Tenant>()),
                Times.Never);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task DeleteAsync_ShouldDeleteTenantAndSave()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                Name = "Acme"
            };

            TenantManager
                .Setup(x => x.Tenants)
                .Returns(
                    QuerableHelper.CreateTenantAsyncQueryable(
                        new[] { tenant })); // <-- IMPORTANT : pas Empty

            TenantManager
                .Setup(x => x.DeleteAsync(It.IsAny<Tenant>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            TenantManager.Verify(
                x => x.DeleteAsync(
                    It.Is<Tenant>(t =>
                        t.Id == 1 &&
                        t.Name == "Acme")),
                Times.Once);

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
