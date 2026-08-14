using IntApplication.DTOs;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;
using IntInfrastructure.Configurations;
using IntInfrastructure.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntApplication.Services
{
    public interface ITenantService
    {
        Task<Tenant> CreateAsync(CreateTenantDto dto);
        Task<Tenant> CreateTenantWithUserAdminAsync(CreateTenantWithUserAdminDto dto);
        Task DeleteAsync(int id);
        Task<Tenant> EditAsync(EditTenantDto dto);
        Task<Tenant> EditTenantWithUserAdminAsync(EditTenantWithUserAdminDto dto);
        Task RemoveAsync(int id);
    }

    public class TenantService : ITenantService
    {
        private readonly ITenantManager TenantManager;
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<Role> RoleManager;
        private readonly IUnitOfWork UnitOfWork;

        private const string TenantAdminRole = "TenantAdmin";

        public TenantService(ITenantManager tenantManager, UserManager<User> userManager, RoleManager<Role> roleManager, IUnitOfWork unitOfWork)
        {
            TenantManager = tenantManager;
            UserManager = userManager;
            RoleManager = roleManager;
            UnitOfWork = unitOfWork;
        }

        public virtual async Task<Tenant> CreateAsync(CreateTenantDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var tenant = new Tenant
            {
                Name = dto.Name
            };

            await TenantManager.CreateAsync(tenant);
            await UnitOfWork.SaveChangesAsync();

            return tenant;
        }

        public virtual async Task<Tenant> CreateTenantWithUserAdminAsync(
            CreateTenantWithUserAdminDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var tenant = new Tenant
            {
                Name = dto.TenantName
            };

            await TenantManager.CreateAsync(tenant);

            await UnitOfWork.SaveChangesAsync();

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                TenantId = tenant.Id
            };

            var userResult = await UserManager.CreateAsync(user, dto.Password);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ", userResult.Errors.Select(x => x.Description)));
            }

            if (!await RoleManager.RoleExistsAsync(TenantAdminRole))
            {
                var role = new Role
                {
                    Name = TenantAdminRole
                };

                var roleResult = await RoleManager.CreateAsync(role);

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", roleResult.Errors.Select(x => x.Description)));
                }
            }

            var roleAssignmentResult =
                await UserManager.AddToRoleAsync(user, TenantAdminRole);

            if (!roleAssignmentResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        roleAssignmentResult.Errors.Select(x => x.Description)));
            }

            await UnitOfWork.SaveChangesAsync();

            return tenant;
        }

        public virtual async Task<Tenant> EditAsync(EditTenantDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var tenant = await TenantManager.Tenants
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (tenant == null)
                throw new KeyNotFoundException(
                    $"Tenant '{dto.Id}' not found.");

            tenant.Name = dto.Name;

            await TenantManager.EditAsync(tenant);
            await UnitOfWork.SaveChangesAsync();

            return tenant;
        }

        public virtual async Task<Tenant> EditTenantWithUserAdminAsync(
            EditTenantWithUserAdminDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var tenant = await TenantManager.Tenants
                .FirstOrDefaultAsync(x => x.Id == dto.TenantId);

            if (tenant == null)
                throw new KeyNotFoundException(
                    $"Tenant '{dto.TenantId}' not found.");

            tenant.Name = dto.TenantName;

            await TenantManager.EditAsync(tenant);

            var user = await UserManager.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.UserId &&
                    x.TenantId == dto.TenantId);

            if (user == null)
                throw new KeyNotFoundException(
                    $"User '{dto.UserId}' not found.");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.Email;

            var userResult = await UserManager.UpdateAsync(user);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        userResult.Errors.Select(x => x.Description)));
            }

            await UnitOfWork.SaveChangesAsync();

            return tenant;
        }

        public virtual async Task RemoveAsync(int id)
        {
            var tenant = await TenantManager.Tenants
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
                throw new KeyNotFoundException(
                    $"Tenant '{id}' not found.");

            await TenantManager.RemoveAsync(tenant);

            await UnitOfWork.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var tenant = await TenantManager.Tenants
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
                throw new KeyNotFoundException(
                    $"Tenant '{id}' not found.");

            await TenantManager.DeleteAsync(tenant);

            await UnitOfWork.SaveChangesAsync();
        }
    }
}
