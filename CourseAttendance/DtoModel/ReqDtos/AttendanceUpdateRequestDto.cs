using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceUpdateRequestDto
	{
		/// <summary>
		/// 打卡类型
		/// </summary>
		[Required]
		public AttendanceStatus Status { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; } = "";
	}
}
