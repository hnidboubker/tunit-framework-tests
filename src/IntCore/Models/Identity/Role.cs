using Microsoft.AspNetCore.Identity;

namespace IntCore.Models.Identity
{
    public class Role : IdentityRole<int>
    {
        public bool IsDeleted { get; set; }
    }
}
