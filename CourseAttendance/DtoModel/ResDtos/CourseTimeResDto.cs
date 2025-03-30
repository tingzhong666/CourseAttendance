using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.DtoModel.ResDtos
{
	/// <summary>
	/// 上课时间段
	/// </summary>
	public class CourseTimeResDto
	{
		/// <summary>
		/// 课程ID
		/// </summary>
		public int CourseId { get; set; }
		/// <summary>
		/// 作息表的  第几节
		/// </summary>
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
	}
}
