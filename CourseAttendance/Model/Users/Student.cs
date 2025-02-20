using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model.Users
{
    public class Student
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; } // 外键，作为主键
        public virtual User User { get; set; } // 对应的用户信息

		[ForeignKey("Grade")]
		public int GradeId { get; set; } // 外键
		public virtual Grade Grade { get; set; } // 对应的班级

		public virtual List<CourseStudent> CourseStudents { get; set; } // 选课
		public virtual List<Attendance> Attendances { get; set; } // 考勤
	}
}
