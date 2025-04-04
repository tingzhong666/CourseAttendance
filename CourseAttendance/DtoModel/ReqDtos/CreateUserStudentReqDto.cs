using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CreateUserStudentReqDto 
	{
		/// <summary>
		/// 班级ID
		/// </summary>
		[Required]
		public required int GradId { get; set; }
	}
}
