
using Microsoft.EntityFrameworkCore;
using OutSmart.Models;

namespace OutSmart.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<User> Users { get; set; }
    
}
