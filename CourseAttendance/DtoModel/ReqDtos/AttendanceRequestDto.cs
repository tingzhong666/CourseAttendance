using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceRequestDto
	{
		public DateTime AttendanceDate { get; set; }
		public DateTime? SignInTime { get; set; }
		public DateTime? SignOutTime { get; set; }
		public AttendanceStatus Status { get; set; }
		public string Remark { get; set; }
		public CheckMethod CheckMethod { get; set; }
		public string Location { get; set; }
		public List<string> AttachmentUrl { get; set; }

		public int CourseId { get; set; }

		public string StudentId { get; set; }
	}
}
