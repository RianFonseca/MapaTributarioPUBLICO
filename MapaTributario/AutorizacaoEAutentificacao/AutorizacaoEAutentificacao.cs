using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace MapaTributario.AutorizacaoEAutentificacao
{
    public class ApplicationUser : IdentityUser
    {

    }
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public IEnumerable<object> User { get; internal set; }
    }
}