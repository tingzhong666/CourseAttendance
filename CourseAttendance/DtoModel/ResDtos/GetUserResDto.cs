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
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
