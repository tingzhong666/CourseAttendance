using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;

namespace CourseAttendance.AppDataContext
{
	public class AppDBContext : IdentityDbContext<User>
	{
		public AppDBContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Student> Students { get; set; }
		public DbSet<Teacher> Teachers { get; set; }
		public DbSet<Academic> Academics { get; set; }
		public DbSet<Admin> Admins { get; set; }
		//public DbSet<Counselor> Counselors { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<CourseStudent> CourseStudents { get; set; }
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Grade> Grades { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// 多对多 考勤  课程与学生
			{
				//modelBuilder.Entity<Attendance>()
				//	.HasKey(cs => new { cs.CourseId, cs.StudentId });

				modelBuilder.Entity<Attendance>()
					.HasOne(u => u.Student)
					.WithMany(u => u.Attendances)
					.HasForeignKey(p => p.StudentId)
				.OnDelete(DeleteBehavior.Restrict);

				modelBuilder.Entity<Attendance>()
					.HasOne(u => u.Course)
					.WithMany(u => u.Attendances)
					.HasForeignKey(p => p.CourseId)
				.OnDelete(DeleteBehavior.Restrict);
			}
			// 多对多 选课  课程与学生
			{
				modelBuilder.Entity<CourseStudent>()
					.HasKey(cs => new { cs.CourseId, cs.StudentId });

				modelBuilder.Entity<CourseStudent>()
					.HasOne(u => u.Student)
					.WithMany(u => u.CourseStudents)
					.HasForeignKey(p => p.StudentId)
					.OnDelete(DeleteBehavior.Restrict);

				modelBuilder.Entity<CourseStudent>()
					.HasOne(u => u.Course)
					.WithMany(u => u.CourseStudents)
					.HasForeignKey(p => p.CourseId)
					.OnDelete(DeleteBehavior.Restrict);
			}

			List<IdentityRole> roles =
			[
				new IdentityRole
				{
					Id="C897C093-BEEA-798C-A116-0DA80A51784A",
					Name = "Admin",
					NormalizedName = "ADMIN"
				},
				new IdentityRole
				{
					Id="33F1A562-4F0B-C638-D624-2E92FE629D4D",
					Name = "Academic",
					NormalizedName = "ACADEMIC"
				},
				new IdentityRole
				{
					Id="523A6FB2-7348-7F06-8DC0-8697BED79A68",
					Name = "Teacher",
					NormalizedName = "TEACHER"
				},
				new IdentityRole
				{
					Id="0F93B20D-BEA0-1628-F7A3-6E9E8A817A4E",
					Name = "Student",
					NormalizedName = "STUDENT"
				},
			];
			modelBuilder.Entity<IdentityRole>().HasData(roles);

			modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
		}

	}
}
