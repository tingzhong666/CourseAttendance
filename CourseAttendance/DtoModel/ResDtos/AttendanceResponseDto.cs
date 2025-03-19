using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class AttendanceResponseDto
	{
		//public DateTime AttendanceDate { get; set; }
		public DateTime? SignInTime { get; set; }
		public AttendanceStatus Status { get; set; }
		public string Remark { get; set; }
		public CheckMethod CheckMethod { get; set; }
		//public string Location { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		//public List<string> AttachmentUrl { get; set; }

		public int CourseId { get; set; }
		public string StudentId { get; set; }
	}
}
