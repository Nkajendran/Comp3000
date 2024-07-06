//This is context part of the code which connects to the table in the database.
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