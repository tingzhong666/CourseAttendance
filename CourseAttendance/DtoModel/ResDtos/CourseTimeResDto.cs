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
		/// 作息表的  第几节 一天中的哪个时间段
		/// </summary>
		public int TimeTableId { get; set; }

		/// <summary>
		/// 日期 哪一天
		/// </summary>
		public DateTime DateDay { get; set; }
	}
}
