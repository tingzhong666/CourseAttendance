using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
    public class Course
	{
        [Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Weekday { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[ForeignKey(nameof(Teacher))]
		public string TeacherId { get; set; }
		public virtual User Teacher { get; set; }
		public virtual List<CourseStudent> CourseStudents { get; set; } // 选课
		public virtual List<Attendance> Attendances { get; set; } // 考勤
	}
}
