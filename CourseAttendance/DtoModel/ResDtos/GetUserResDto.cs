using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class GetUserResDto
	{
		[Required]
		public required string Id { get; set; }
		[Required]
		public required string Name { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		[Required]
		public required string UserName { get; set; }
		[Required]
		public required List<UserRole> Roles { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }

		public GetAcademicResDto? GetAcademicExt { get; set; }
		public GetAdminResDto? GetAdminExt { get; set; }
		public GetStudentResDto? GetStudentExt { get; set; }
		public GetTeacherResDto? GetTeacherExt { get; set; }

	}
}
