using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class AttendanceResponseDto
	{
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		[Required]
		public int Id { get; set; }
		[Required]
		public CheckMethod CheckMethod { get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		/// <summary>
		/// 结束时间
		/// </summary>
		[Required]
		public DateTime EndTime { get; set; }
		[Required]
		public AttendanceStatus Status { get; set; }
		public DateTime? SignInTime { get; set; }
		[Required]
		public string Remark { get; set; }


		[Required]
		public int CourseId { get; set; }
		[Required]
		public string StudentId { get; set; }
	}
}
