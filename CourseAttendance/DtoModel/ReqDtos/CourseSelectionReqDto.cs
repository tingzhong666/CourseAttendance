using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseSelectionReqDto
	{
		/// <summary>
		/// 上课表现
		/// </summary>
		public PerformanceLevel Performance { get; set; } = PerformanceLevel.None;
	}
}
