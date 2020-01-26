using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Github2Wandbox.Repository
{
    public class PublicationsContext : IdentityDbContext
    {
        public DbSet<Publication> Publications { get; set; }

        public PublicationsContext(DbContextOptions<PublicationsContext> options) : base(options)
        {
        }
    }
}
