using Microsoft.EntityFrameworkCore;
using MyCarManagementApplication.Models;

namespace MyCarManagementApplication.Data
{
    public class CarDbContext : DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
