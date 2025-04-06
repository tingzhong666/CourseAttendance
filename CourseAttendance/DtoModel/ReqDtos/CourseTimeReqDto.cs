using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseTimeReqDto
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
