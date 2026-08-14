using IntCore.Models.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace IntCore.Models.Identity
{
    public class User : IdentityUser<int>
    {

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;


        public string Status { get; set; } = default!;
        public string Avatar { get; set; } = default!;

        public int? TenantId { get; set; }
        public virtual Tenant? Tenant { get; set; }
    }
}
