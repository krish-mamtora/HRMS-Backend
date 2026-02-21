using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Achievements;
//using HRMS_Backend.Entities.FixEntityUserProfile;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Entities.FixEntityUserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using HRMS_Backend.Migrations;
using TravelDocuments = HRMS_Backend.Entities.TravelandExpense.TravelDocuments;

namespace HRMS_Backend.Data
{
    public class MyDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jobs>() 
                .HasOne(j=>j.User)
                .WithMany()
                .HasForeignKey(j => j.ManagedBy)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<TravelDocuments>()
            //    .HasOne(td=>td.TravelPlan)
            //    .WithMany(td => td.TravelDocuments)
            //    .HasForeignKey(td => td.TravelPlanId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referals>()
                .HasOne(r=>r.Job)
                .WithMany(r=>r.Referals)
                .HasForeignKey(r=>r.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Referals>()
                .HasOne(r=>r.User)
                .WithMany()
                .HasForeignKey(r=>r.EmpId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShareEmail>()
                .HasOne(r => r.Job)
                .WithMany(r => r.ShareEmail)
                .HasForeignKey(r => r.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShareEmail>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.EmpId)
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

            modelBuilder.Entity<Expenses>()
                .HasOne(ep=>ep.TravelPlan)
                .WithMany(ep=>ep.Expenses)
                .HasForeignKey(ep=>ep.TravelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expenses>()
                .HasOne(ep=>ep.User)
                .WithMany()
                .HasForeignKey(ep=>ep.EmplId)
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
        public DbSet<FileModel2> FileModel2 { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<ShareEmail> ShareEmail { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<TravelExpense>TravelExpense { get; set; }
        public DbSet<ExpenseProof> ExpenseProof { get; set; }
        public DbSet<TravelDocuments> TravelDocuments { get; set; }

        public DbSet<TravelAssignEmail> TravelAssignEmail { get; set; }

    }

}
