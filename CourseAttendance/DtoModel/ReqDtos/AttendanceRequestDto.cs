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
		//public DateTime? SignInTime { get; set; } 用服务器时间
		//public DateTime? SignOutTime { get; set; }
		//public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent; //服务器逻辑
		//public string Remark { get; set; }
		//[Required]
		//public CheckMethod CheckMethod { get; set; } 这个不改变
		//public string Location { get; set; }
		//public List<string> AttachmentUrl { get; set; }

		[Required]
		public int CourseId { get; set; }
		[Required]
		public string StudentId { get; set; }

		/// <summary>
		/// 密码 密码打卡用
		/// </summary>
		public string? PassWord { get; set; } = "";
	}
}
