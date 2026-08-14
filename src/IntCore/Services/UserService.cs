////using IntCore.DTOs;
////using IntCore.Models.Identity;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.EntityFrameworkCore;

////namespace IntCore.Services
////{
////    public class UserService
////    {
////        private readonly UserManager<User> UserManager;

////        public UserService(UserManager<User> userManager)
////        {
////            UserManager = userManager;
////        }


////        public virtual async Task<IReadOnlyList<UserDto>> GetUsersAsync()
////        {
////            var users = await UserManager.Users.ToListAsync();

////            var dto = new List<UserDto>();

////            foreach (var user in users)
////            {
////                var roles = await UserManager.GetRolesAsync(user);

////                dto.Add(new UserDto
////                {
////                    Id = user.Id,
////                    FirstName = user.FirstName,
////                    LastName = user.LastName,
////                    FullName = $"{user.FirstName} {user.LastName}",
////                    Email = user.Email,
////                    Tenant = user.Tenant?.Name ?? "",
////                    Roles = roles.ToArray()
////                });
////            }

////            return dto;
////        }

////        public virtual async Task<IdentityResult> CreateAsync(CreateUserDto dto)
////        {
////            var user = new User
////            {
////                FirstName = dto.FirstName,
////                LastName = dto.LastName,
////                Email = dto.Email,
////                UserName = dto.UserName,
////                TenantId = dto.TenantId
////            };
////            var result = await UserManager.CreateAsync(user, dto.Password);
////            if (!result.Succeeded)
////            {
////                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
////            }
////            if (dto.Roles != null && dto.Roles.Length > 0)
////            {
////                await UserManager.AddToRolesAsync(user, dto.Roles);
////            }

////            return IdentityResult.Success;
////        }
////    }
////}
