using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class LoginRes
	{
		[Required]
		public string Token { get; set; }
		[Required]
		public string UserName { get; set; }
	}
}
