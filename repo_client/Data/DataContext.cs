using Microsoft.EntityFrameworkCore;
using repo_client.Models;

namespace repo_client.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>().HasKey(x => x.Id);
        }
    }
}
