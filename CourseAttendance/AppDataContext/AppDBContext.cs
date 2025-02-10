using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;

namespace CourseAttendance.AppDataContext
{
	public class AppDBContext : DbContext
	{
		public AppDBContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<CourseStudent> CourseStudents { get; set; }
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Model.Attachment> Attachments { get; set; }
		public DbSet<Grade> Grades { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CourseStudent>()
				.HasKey(cs => new { cs.CourseId, cs.StudentId });
		}

	}
}
