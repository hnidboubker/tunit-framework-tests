using IntCore.Models.Identity;

namespace IntCore.Models.MultiTenancy
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
