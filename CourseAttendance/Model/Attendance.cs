using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Enums;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
    public class Attendance
	{
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
		public string AttachmentUrl { get; set; } 
		

		[ForeignKey("Course")]
		public int CourseId { get; set; }
		public virtual Course Course { get; set; }
		[ForeignKey("Student")]
		public string StudentId { get; set; }
		public virtual Student Student { get; set; }
	}
}
