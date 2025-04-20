using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class PasswordAttendanceRequest
	{
		[Required]
		public string Password { get; set; }
	}
}
