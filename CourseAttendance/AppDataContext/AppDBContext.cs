using CourseAttendance.Enums;
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
		public DbSet<WebSystemConfig> WebSystemConfigs { get; set; }
		public DbSet<TimeTable> TimeTables { get; set; }
		public DbSet<CourseTime> CourseTimes { get; set; }
		public DbSet<MajorsCategory> MajorsCategories { get; set; }
		public DbSet<MajorsSubcategory> MajorsSubcategories { get; set; }
		public DbSet<AttendanceBatch> AttendanceBatchs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// 多对多 考勤  课程与学生
			{
				//modelBuilder.Entity<Attendance>()
				//	.HasKey(cs => new { cs.CourseId, cs.StudentId });

				//modelBuilder.Entity<Attendance>()
				//	.HasOne(u => u.Student)
				//	.WithMany(u => u.Attendances)
				//	.HasForeignKey(p => p.StudentId)
				//.OnDelete(DeleteBehavior.Restrict);

				//modelBuilder.Entity<Attendance>()
				//	.HasOne(u => u.Course)
				//	.WithMany(u => u.Attendances)
				//	.HasForeignKey(p => p.CourseId)
				//.OnDelete(DeleteBehavior.Restrict);

				modelBuilder.Entity<Student>()
					.HasMany(x => x.Attendances)
					.WithOne(x => x.Student)
					.HasForeignKey(x => x.StudentId)
					.OnDelete(DeleteBehavior.ClientCascade);
			}
			// 多对多 选课  课程与学生
			{
				modelBuilder.Entity<CourseStudent>()
					.HasKey(cs => new { cs.CourseId, cs.StudentId });

				modelBuilder.Entity<CourseStudent>()
					.HasOne(u => u.Student)
					.WithMany(u => u.CourseStudents)
					.HasForeignKey(p => p.StudentId)
					.OnDelete(DeleteBehavior.ClientCascade);

				modelBuilder.Entity<CourseStudent>()
					.HasOne(u => u.Course)
					.WithMany(u => u.CourseStudents)
					.HasForeignKey(p => p.CourseId)
					.OnDelete(DeleteBehavior.ClientCascade);
			}

			// 多对多 上课的时间 课程与作息表
			{
				//modelBuilder.Entity<CourseTime>()
				//	.HasKey(ct => new { ct.CourseId, ct.TimeTableId });

				//modelBuilder.Entity<CourseTime>()
				//	.HasOne(u => u.Course)
				//	.WithMany(u => u.CourseTimes)
				//	.HasForeignKey(p => p.CourseId)
				//	.OnDelete(DeleteBehavior.Restrict);

				//modelBuilder.Entity<CourseTime>()
				//	.HasOne(u => u.TimeTable)
				//	.WithMany(u => u.CourseTimes)
				//	.HasForeignKey(p => p.TimeTableId)
				//	.OnDelete(DeleteBehavior.Restrict);
			}
			// 身份权限
			var roles = new List<IdentityRole>()
			{
				new IdentityRole
				{
					Id="2325ECCC-C9B5-6026-BDDB-DF35C7761CE2",
					Name = UserRole.Admin.ToString(),
					NormalizedName = UserRole.Admin.ToString()?.ToUpperInvariant() ?? ""
				},
				new IdentityRole
				{
					Id="A68C4FE1-8F75-E515-2E4C-8AB1E2C814F1",
					Name = UserRole.Academic.ToString(),
					NormalizedName = UserRole.Academic.ToString()?.ToUpperInvariant() ?? ""
				},
				new IdentityRole
				{
					Id="BA576549-1FE7-4A5E-F4E1-FA359040BD55",
					Name = UserRole.Teacher.ToString(),
					NormalizedName = UserRole.Teacher.ToString()?.ToUpperInvariant() ?? ""
				},
				new IdentityRole
				{
					Id="B7E953C4-6438-0AAB-8894-602E2A5BF1AD",
					Name = UserRole.Student.ToString(),
					NormalizedName = UserRole.Student.ToString()?.ToUpperInvariant() ?? ""
				},
			};
			//foreach (var x in Enum.GetValues(typeof(UserRole)))
			//{
			//	roles.Add(new IdentityRole
			//	{
			//		//Id = Guid.NewGuid().ToString(),
			//		Name = x.ToString(),
			//		NormalizedName = x.ToString()?.ToUpperInvariant() ?? ""
			//	});
			//}
			modelBuilder.Entity<IdentityRole>().HasData(roles);

			// 用户的工号唯一
			modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
		}

	}
}
