using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Enums;

namespace CourseAttendance.Model
{
	public class Attendance
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public int StudentId { get; set; }
		public DateTime AttendanceDate { get; set; }
		public DateTime? SignInTime { get; set; }
		public DateTime? SignOutTime { get; set; }
		public AttendanceStatus Status { get; set; }
		public PerformanceLevel Performance { get; set; }
		public string Remark { get; set; }
		public CheckMethod CheckMethod { get; set; }
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		public virtual Course Course { get; set; }
		public virtual User Student { get; set; }
	}
}
