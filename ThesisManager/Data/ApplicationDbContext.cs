using Microsoft.EntityFrameworkCore;
using ThesisManager.Models;

namespace ThesisManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<NumOfStep> NumOfSteps => Set<NumOfStep>();
        public DbSet<Supervisor> Supervisors => Set<Supervisor>();
        public DbSet<StudentGuide> StudentGuides => Set<StudentGuide>();
        public DbSet<DissertationDraft> DissertationDrafts => Set<DissertationDraft>();
        public DbSet<Dissertation> Dissertations => Set<Dissertation>();
        public DbSet<ThesisDefence> ThesisDefences => Set<ThesisDefence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().ToTable("student");
            modelBuilder.Entity<NumOfStep>().ToTable("num_of_step");
            modelBuilder.Entity<Supervisor>().ToTable("supervisor");
            modelBuilder.Entity<StudentGuide>().ToTable("student_guide");
            modelBuilder.Entity<DissertationDraft>().ToTable("dissertation_draft");
            modelBuilder.Entity<Dissertation>().ToTable("dissertation");
            modelBuilder.Entity<ThesisDefence>().ToTable("theses_defence");

            // PostgreSQL requires explicit identity columns:
            modelBuilder.Entity<Student>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<NumOfStep>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<Supervisor>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<StudentGuide>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<DissertationDraft>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<Dissertation>().Property(p => p.Id).UseIdentityAlwaysColumn();
            modelBuilder.Entity<ThesisDefence>().Property(p => p.Id).UseIdentityAlwaysColumn();

            // Seed num_of_step
            /*modelBuilder.Entity<NumOfStep>().HasData(
                new NumOfStep { Id = 1, Name = "تقديم الفكر" },
                new NumOfStep { Id = 2, Name = "حطاب تعيين المشرف" },
                new NumOfStep { Id = 3, Name = "قرار المناقشة" }
            );*/
        }
    }
}