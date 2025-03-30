using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.Model
{
	/// <summary>
	/// 课程的上课时间 多对多关系
	/// </summary>
	public class CourseTime
	{
		/// <summary>
		/// 课程ID
		/// </summary>
		[ForeignKey("Course")]
		public int CourseId { get; set; }
		/// <summary>
		/// 作息表的  第几节
		/// </summary>
		[ForeignKey("TimeTable")]
		public int TimeTableId { get; set; }

		/// <summary>
		/// 周几
		/// </summary>
		public Weekday Weekday { get; set; }
		/// <summary>
		/// 起始日期
		/// </summary>
		public DateTime StartTime { get; set; }
		/// <summary>
		/// 终末日期
		/// </summary>
		public DateTime EndTime { get; set; }

		public virtual Course Course { get; set; }
		public virtual TimeTable TimeTable { get; set; }
	}
}
