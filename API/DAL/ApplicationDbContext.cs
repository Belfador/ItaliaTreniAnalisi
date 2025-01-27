using API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Sample> Samples { get; set; }
    }
}
