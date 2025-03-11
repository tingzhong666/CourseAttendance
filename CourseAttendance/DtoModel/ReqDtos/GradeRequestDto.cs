using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class GradeRequestDto
	{
		[Required]
		public string Name { get; set; }

		// 辅导员ID
		// public int CounselorId { get; set; }
	}
}
