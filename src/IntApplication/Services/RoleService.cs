using IntCore.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace IntApplication.Services
{
    public class RoleService
    {
        private readonly RoleManager<Role> RoleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            RoleManager = roleManager;
        }
    }
}
