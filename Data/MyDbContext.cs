using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.JobListing;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HRMS_Backend.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users{ get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<GameConfiguration> GameConfiguration { get; set; }
        public DbSet<GameSlots> GameSlots { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
    }
}
