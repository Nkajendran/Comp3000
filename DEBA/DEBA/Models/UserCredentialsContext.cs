using DEBA.Models;
using Microsoft.EntityFrameworkCore;

namespace UserCredentialsModels
{
    public class UserCredentialsContext : DbContext
    {
        public DbSet<UserCredentialsController> UserCredentials { get; set; }
        public UserCredentialsContext(DbContextOptions<UserCredentialsContext> options) : base(options)
        {

        }
    }
}