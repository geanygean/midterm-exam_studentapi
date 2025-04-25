using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

public class ApplicationDbContext : DbContext
{
	public DbSet<Student> Students { get; set; }
	public DbSet<Section> Sections { get; set; }
	public DbSet<Subject> Subjects { get; set; }
	public DbSet<StudentSection> StudentSections { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{ }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<StudentSection>()
			.HasKey(ss => new { ss.StudentId, ss.SectionId });

		modelBuilder.Entity<StudentSection>()
			.HasOne(ss => ss.Student)
			.WithMany(s => s.StudentSections)
			.HasForeignKey(ss => ss.StudentId);

		modelBuilder.Entity<StudentSection>()
			.HasOne(ss => ss.Section)
			.WithMany(s => s.StudentSections)
			.HasForeignKey(ss => ss.SectionId);
	}
}
