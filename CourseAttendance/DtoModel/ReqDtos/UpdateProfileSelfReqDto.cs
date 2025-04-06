using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class UpdateProfileSelfReqDto
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
		public string Name { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		[Required]
		public string UserName { get; set; }
		public CreateUserAcademicReqDto? CreateAcademicExt { get; set; }
		public CreateUserAdminReqDto? CreateAdminExt { get; set; }
		public CreateUserStudentReqDto? CreateStudentExt { get; set; }
		public CreateUserTeacherReqDto? CreateTeacherExt { get; set; }
	}
}
