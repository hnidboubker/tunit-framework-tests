using IntApplication.Services;
using IntCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IntApplication.UnitTests.Builders
{
    public sealed class UserServiceBuilder
    {
        private readonly Mock<UserManager<User>> _userManager = CreateUserManagerMock();
        private readonly Mock<RoleManager<Role>> _roleManager = CreateRoleManagerMock();


        public Mock<UserManager<User>> UserManager => _userManager;

        public Mock<RoleManager<Role>> RoleManager => _roleManager;



        public UserService Build()
        {
            return new UserService(
                UserManager.Object,
                RoleManager.Object
                );
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
                new PasswordHasher<User>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                Mock.Of<IServiceProvider>(),
                new Mock<ILogger<UserManager<User>>>().Object);
        }

        private static Mock<RoleManager<Role>> CreateRoleManagerMock()
        {
            var store = new Mock<IRoleStore<Role>>();

            return new Mock<RoleManager<Role>>(
                store.Object,
                new List<IRoleValidator<Role>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<Role>>>().Object);
        }
    }
}
