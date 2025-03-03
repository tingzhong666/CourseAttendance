using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class ChangePasswordReqDto
	{
		[Required]
		public required string NewPassword { get; set; }
		[Required]
		public required string ConfirmPassword { get; set; }
		[Required]
		public required string CurrentPassword { get; set; }
		[Required]
		public required string UserId { get; set; }
	}
}
