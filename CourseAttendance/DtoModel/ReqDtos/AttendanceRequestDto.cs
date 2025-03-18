using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	/// <summary>
	/// 学生打卡请求用
	/// </summary>
	public class AttendanceRequestDto
	{
		//public DateTime AttendanceDate { get; set; }
		public DateTime? SignInTime { get; set; }
		//public DateTime? SignOutTime { get; set; }
		public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
		public string Remark { get; set; }
		[Required]
		public CheckMethod CheckMethod { get; set; }
		public string Location { get; set; }
		public List<string> AttachmentUrl { get; set; }

		public int CourseId { get; set; }

		public string StudentId { get; set; }
	}
}
