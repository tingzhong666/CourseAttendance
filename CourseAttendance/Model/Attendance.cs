using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Enums;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
    public class Attendance
	{
		//public DateTime AttendanceDate { get; set; }
		/// <summary>
		/// 签到时间
		/// </summary>
		public DateTime? SignInTime { get; set; }
		//public DateTime? SignOutTime { get; set; }
		/// <summary>
		/// 考勤状态
		/// </summary>
		public AttendanceStatus Status { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }
		/// <summary>
		/// 打卡类型
		/// </summary>
		public CheckMethod CheckMethod { get; set; }
		/// <summary>
		/// 地点
		/// </summary>
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		/// <summary>
		/// 附件
		/// </summary>
		public List<string> AttachmentUrl { get; set; } 
		

		[ForeignKey("Course")]
		public int CourseId { get; set; }
		public virtual Course Course { get; set; }
		[ForeignKey("Student")]
		public string StudentId { get; set; }
		public virtual Student Student { get; set; }
	}
}
