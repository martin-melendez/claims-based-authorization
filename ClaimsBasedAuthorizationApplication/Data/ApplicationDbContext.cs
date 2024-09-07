using ClaimsBasesAuthorizationApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ClaimsBasesAuthorizationApplication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}