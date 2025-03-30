using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class TimeTableResDto
	{
		public int Id { get; set; }
		/// <summary>
		/// 描述名称
		/// </summary>
		public string Name { get; set; }
		public TimeSpan Start { get; set; }
		public TimeSpan End { get; set; }
	}
}
