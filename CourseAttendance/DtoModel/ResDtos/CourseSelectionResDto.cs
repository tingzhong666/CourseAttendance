using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class CourseSelectionResDto
	{
		/// <summary>
		/// 上课表现
		/// </summary>
		public PerformanceLevel Performance { get; set; }
		public string StudentId { get; set; }
		public int CourseId { get; set; }
	}
}
