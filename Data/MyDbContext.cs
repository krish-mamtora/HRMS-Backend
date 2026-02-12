using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;

namespace HRMS_Backend.Data
{
    public class MyDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Jobs>()
                .HasOne(j=>j.User)
                .WithMany()
                .HasForeignKey(j => j.ManagedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referals>()
                .HasOne<Jobs>(r=>r.Job)
                .WithMany()
                .HasForeignKey(r=>r.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Referals>()
                .HasOne(r=>r.Employee)
                .WithMany()
                .HasForeignKey(r=>r.EmpId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<TravelPlan>()
                .HasOne(tp=>tp.User)
                .WithMany()
                .HasForeignKey(tp=>tp.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelAssignment>()
                .HasOne(tp=>tp.TravelPlan)
                .WithMany(tp=>tp.TravelAssignment)
                .HasForeignKey(ta=>ta.PId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelAssignment>()
                .HasOne(ta => ta.User)
                .WithMany()
                .HasForeignKey(ta => ta.EmpId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users{ get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<GameConfiguration> GameConfiguration { get; set; }
        public DbSet<GameSlots> GameSlots { get; set; }
        public DbSet<Jobs> Jobs { get; set; }

        public DbSet<Tags> Tags { get; set; }
      
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TravelPlan> TravelPlan { get; set; }

        public DbSet<Referals> Referals { get; set; }
        public DbSet<TravelAssignment> TravelAssignment { get; set; }
    }

}
