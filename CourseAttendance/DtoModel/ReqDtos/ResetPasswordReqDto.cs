using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class ResetPasswordReqDto
	{
		[Required]
		public required string NewPassword { get; set; }
		[Required]
		public required string ConfirmPassword { get; set; }
		[Required]
		public required string UserId { get; set; }
	}
}
