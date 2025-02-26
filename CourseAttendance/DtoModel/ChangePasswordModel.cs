using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel
{
	public class ChangePasswordModel
	{
		[Required]
		public string NewPassword { get; set; }
		[Required]
		public string ConfirmPassword { get; set; }
		[Required]
		public string CurrentPassword { get; set; }
	}
}