// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ThesisManager.Models;

namespace ThesisManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StepDefinition> StepDefinitions => Set<StepDefinition>();
        public DbSet<Thesis> Theses => Set<Thesis>();
        public DbSet<StepHistory> StepHistory => Set<StepHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(p => p.Id).UseIdentityAlwaysColumn();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Role);
            });

            // Configure Track entity
            modelBuilder.Entity<Track>(entity =>
            {
                entity.ToTable("tracks");
                entity.Property(p => p.Id).UseIdentityAlwaysColumn();
                entity.HasIndex(t => t.Name).IsUnique();
            });

            // Configure StepDefinition entity
            modelBuilder.Entity<StepDefinition>(entity =>
            {
                entity.ToTable("step_definitions");
                entity.HasKey(e => e.StepNumber);
            });

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");
                entity.Property(p => p.Id).UseIdentityAlwaysColumn();
                entity.HasIndex(s => s.StudentId).IsUnique();
                entity.HasIndex(s => s.CurrentStep);
                entity.HasIndex(s => s.Status);
                entity.HasIndex(s => s.TutorId);
                entity.HasIndex(s => s.SupervisorId);

                // Relationships
                entity.HasOne(s => s.Tutor)
                    .WithMany(u => u.TutoredStudents)
                    .HasForeignKey(s => s.TutorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.Supervisor)
                    .WithMany(u => u.SupervisedStudents)
                    .HasForeignKey(s => s.SupervisorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.Thesis)
                    .WithOne(t => t.Student)
                    .HasForeignKey<Thesis>(t => t.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Thesis entity
            modelBuilder.Entity<Thesis>(entity =>
            {
                entity.ToTable("theses");
                entity.Property(p => p.Id).UseIdentityAlwaysColumn();
                entity.HasIndex(t => t.StudentId).IsUnique();
                entity.HasIndex(t => t.DegreeType);
                entity.HasIndex(t => t.TrackId);

                // Configure JSONB column for defense committee
                entity.Property(t => t.DefenseCommittee)
                    .HasColumnType("jsonb");

                // Relationship with Track
                entity.HasOne(t => t.Track)
                    .WithMany(tr => tr.Theses)
                    .HasForeignKey(t => t.TrackId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure StepHistory entity
            modelBuilder.Entity<StepHistory>(entity =>
            {
                entity.ToTable("step_history");
                entity.Property(p => p.Id).UseIdentityAlwaysColumn();
                entity.HasIndex(sh => sh.StudentId);
                entity.HasIndex(sh => sh.Status);
                entity.HasIndex(sh => new { sh.StudentId, sh.StepNumber }).IsUnique();

                // Relationships
                entity.HasOne(sh => sh.Student)
                    .WithMany(s => s.StepHistories)
                    .HasForeignKey(sh => sh.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sh => sh.StepDefinition)
                    .WithMany(sd => sd.StepHistories)
                    .HasForeignKey(sh => sh.StepNumber)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}