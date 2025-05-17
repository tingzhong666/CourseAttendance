using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Model.Users;
using CourseAttendance.Enums;

namespace CourseAttendance.Model
{
    public class Course
	{
        [Key]
		public int Id { get; set; }
		[MaxLength(10)]
		public string Name { get; set; }
		[MaxLength(10)]
		public string Location { get; set; }


		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[ForeignKey(nameof(Teacher))]
		public string TeacherUserId { get; set; }
		public virtual Teacher Teacher { get; set; }

		// 小专业
		[ForeignKey(nameof(MajorsSubcategory))]
		public int MajorsSubcategoryId { get; set; }
		public virtual MajorsSubcategory MajorsSubcategory { get; set; }


		public virtual List<CourseStudent> CourseStudents { get; set; } // 选课
		public virtual List<AttendanceBatch> AttendanceBatchs { get; set; } // 考勤

		/// <summary>
		/// 上课时间
		/// </summary>
		public virtual List<CourseTime> CourseTimes {  get; set; }
	}
}
