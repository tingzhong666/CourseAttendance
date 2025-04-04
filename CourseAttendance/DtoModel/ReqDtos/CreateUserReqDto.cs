using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CreateUserReqDto
	{
		/// <summary>
		/// 邮件
		/// </summary>
		public string? Email { get; set; }
		/// <summary>
		/// 手机号
		/// </summary>
		public string? Phone { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		[Required]
		public required string Name { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		[Required]
		public required string UserName { get; set; }
		/// <summary>
		/// 密码
		/// </summary>
		[Required]
		public required string PassWord { get; set; }
		[Required]
		public required List<UserRole> Roles { get; set; }

		public CreateUserAcademicReqDto? CreateAcademicExt { get; set; }
		public CreateUserAdminReqDto? CreateAdminExt { get; set; }
		public CreateUserStudentReqDto? CreateStudentExt { get; set; }
		public CreateUserTeacherReqDto? CreateTeacherExt { get; set; }
	}
}
