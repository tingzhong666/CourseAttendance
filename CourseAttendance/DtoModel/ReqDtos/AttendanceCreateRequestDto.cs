using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	/// <summary>
	/// 创建打卡用
	/// </summary>
	public class AttendanceCreateRequestDto
	{
		/// <summary>
		/// 密码 密码打卡用
		/// </summary>
		public string? PassWord { get; set; } = "";
		/// <summary>
		/// 结束时间
		/// </summary>
		[Required]
		public DateTime EndTime { get; set; }
		[Required]
		public int CourseId { get; set; }
		/// <summary>
		/// 打卡类型
		/// </summary>
		[Required]
		public CheckMethod CheckMethod { get; set; }
	}
}
