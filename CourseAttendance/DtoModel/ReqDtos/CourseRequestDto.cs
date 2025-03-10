using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseRequestDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Weekday { get; set; }

		[Required]
		public DateTime StartTime { get; set; }

		[Required]
		public DateTime EndTime { get; set; }

		[Required]
		public string Location { get; set; }

		[Required]
		public string TeacherId { get; set; }
	}
}
