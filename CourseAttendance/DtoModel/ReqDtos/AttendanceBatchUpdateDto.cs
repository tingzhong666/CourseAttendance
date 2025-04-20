using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceBatchUpdateDto
	{
		[Required]
		public int Id{ get; set; }


		/// <summary>
		/// 打卡状态
		/// </summary>
		public CheckMethod? CheckMethod { get; set; }
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? EndTime { get; set; }


		/// <summary>
		/// 密码 密码打卡用 这里只是用作打卡的数字验证 不用加密
		/// </summary>
		public string? PassWord { get; set; } = "";
	}
}
