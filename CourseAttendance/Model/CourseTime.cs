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
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 课程ID
		/// </summary>
		[ForeignKey("Course")]
		public int CourseId { get; set; }
		/// <summary>
		/// 作息表的  第几节 一天中的哪个时间段
		/// </summary>
		[ForeignKey("TimeTable")]
		public int TimeTableId { get; set; }
		/// <summary>
		/// 日期 哪一天
		/// </summary>
		public DateTime DateDay { get; set; }

		public virtual Course Course { get; set; }
		public virtual TimeTable TimeTable { get; set; }
	}
}
