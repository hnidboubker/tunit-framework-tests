using IntApplication.DTOs;
using IntCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntApplication.Services
{
    public interface IUserService
    {
        Task<IdentityResult> CreateAsync(CreateUserDto dto);
        Task<IReadOnlyList<UserDto>> GetUsersAsync();
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<Role> RoleManager;
        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }


        public virtual async Task<IReadOnlyList<UserDto>> GetUsersAsync()
        {
            var users = await UserManager.Users.ToListAsync();

            var dto = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await UserManager.GetRolesAsync(user);

                dto.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Tenant = user.Tenant?.Name ?? "",
                    Roles = roles.ToArray()
                });
            }

            return dto;
        }

        public virtual async Task<IdentityResult> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                TenantId = dto.TenantId
            };
            var result = await UserManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            if (dto.Roles != null && dto.Roles.Length > 0)
            {
                await UserManager.AddToRolesAsync(user, dto.Roles);
            }

            return IdentityResult.Success;
        }
    }
}
