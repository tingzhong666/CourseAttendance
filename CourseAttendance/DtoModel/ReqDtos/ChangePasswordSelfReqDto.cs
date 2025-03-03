using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class ChangePasswordSelfReqDto
	{
		[Required]
		public string NewPassword { get; set; }
		[Required]
		public string ConfirmPassword { get; set; }
		[Required]
		public string CurrentPassword { get; set; }
	}
}