using CourseAttendance.Enums;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
	/// <summary>
	/// 选课表实体
	/// </summary>
    public class CourseStudent
	{
		public int CourseId { get; set; }
		public string StudentId { get; set; }

		public virtual Course Course { get; set; }
		public virtual Student Student { get; set; }

		/// <summary>
		/// 上课表现
		/// </summary>
		public PerformanceLevel Performance { get; set; }
	}
}
