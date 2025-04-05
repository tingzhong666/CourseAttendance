using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CreateUserReqDto: UpdateProfileReqDto
	{
		/// <summary>
		/// 密码
		/// </summary>
		[Required]
		public required string PassWord { get; set; }
	}
}
