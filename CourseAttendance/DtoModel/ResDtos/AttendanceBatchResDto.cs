using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class AttendanceBatchResDto
	{
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public DateTime UpdatedAt { get; set; }

		[Required]
		public int Id { get; set; }
		[Required]
		public CheckMethod CheckMethod { get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		[Required]
		public DateTime EndTime { get; set; }

		public string? PassWord { get; set; }
		public string? QRCode { get; set; }


		[Required]
		public int CourseId { get; set; }
		[Required]
		public List<int> AttendanceIds { get; set; }
	}
}
