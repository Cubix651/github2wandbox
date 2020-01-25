using Microsoft.EntityFrameworkCore;

namespace Github2Wandbox.Repository
{
    public class PublicationsContext : DbContext
    {
        public DbSet<Publication> Publications { get; set; }

        public PublicationsContext(DbContextOptions<PublicationsContext> options) : base(options)
        {
        }
    }
}
