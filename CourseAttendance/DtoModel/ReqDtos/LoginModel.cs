using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class LoginModel
	{
		/// <summary>
		/// 工号
		/// </summary>
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
