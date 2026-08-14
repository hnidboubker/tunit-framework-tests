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

        public virtual async Task<IdentityResult> EditAsync(int userId, EditUserDto dto)


        {
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Utilisateur introuvable."
                    });
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.UserName;
            user.TenantId = dto.TenantId;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return result;
            }

            // Gestion des rôles
            if (dto.Roles != null)
            {
                var currentRoles = await UserManager.GetRolesAsync(user);

                var rolesToRemove = currentRoles
                    .Except(dto.Roles)
                    .ToArray();

                var rolesToAdd = dto.Roles
                    .Except(currentRoles)
                    .ToArray();

                if (rolesToRemove.Length > 0)
                {
                    var removeResult =
                        await UserManager.RemoveFromRolesAsync(user, rolesToRemove);

                    if (!removeResult.Succeeded)
                    {
                        return removeResult;
                    }
                }

                if (rolesToAdd.Length > 0)
                {
                    var addResult =
                        await UserManager.AddToRolesAsync(user, rolesToAdd);

                    if (!addResult.Succeeded)
                    {
                        return addResult;
                    }
                }
            }

            // Changement de mot de passe uniquement s'il est fourni
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult =
                    await UserManager.ResetPasswordAsync(
                        user,
                        token,
                        dto.Password);

                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(int userId)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Utilisateur introuvable."
                    });
            }

            return await UserManager.DeleteAsync(user);
        }

        public virtual async Task<IdentityResult> RemoveAsync(int userId, string[] roles)


        {
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Utilisateur introuvable."
                    });
            }

            if (roles == null || roles.Length == 0)
            {
                return IdentityResult.Success;
            }

            return await UserManager.RemoveFromRolesAsync(user, roles);
        }
    }
}

