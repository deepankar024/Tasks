using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FeedbackFormWebApp.Models;

namespace FeedbackFormWebApp.Models
{
    public class FeedbackDbContext : DbContext  //inherit EF
    {
        public FeedbackDbContext() : base("name=FeedbackDbContext")
        {
            // Enable automatic migrations for development
            Database.SetInitializer(new CreateDatabaseIfNotExists<FeedbackDbContext>());
        }

        public DbSet<Feedback> Feedbacks { get; set; }   //representscollection of all Feedback records  db.Feedbacks.ToList()

        protected override void OnModelCreating(DbModelBuilder modelBuilder)  //Fluent API use kiya basically annotation hai OnModelCreating() for complex configurations
        {
            // Remove pluralizing table name convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();   // feedbackss eg

            // Configure Feedback entity
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Email)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Category)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Message)
                .IsRequired()
                .HasMaxLength(1000);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.SubmittedAt)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}

// This class is the bridge between your C# models and the database. It inherits from Entity Framework's DbContext.