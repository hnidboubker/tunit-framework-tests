using IntCore.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntEntityFrameworkCore.Persistence
{
    public class DefaultContext : IdentityDbContext<User, Role, int>
    {
        public DefaultContext(DbContextOptions<DefaultContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurações adicionais das entidades, se necessário.
        }
    }
}
