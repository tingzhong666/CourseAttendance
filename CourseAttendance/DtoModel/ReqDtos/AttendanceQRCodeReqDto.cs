using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceQRCodeReqDto
	{
		// 二维码解码的随机字符串
		[Required]
		public string Code { get; set; }
	}
}
