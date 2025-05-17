using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.Model
{
	public class AttendanceBatch
	{
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;


		[Key]
		public int Id { get; set; }


		/// <summary>
		/// 打卡类型
		/// </summary>
		public CheckMethod CheckMethod { get; set; }
		/// <summary>
		/// 开始时间
		/// </summary>
		[Column(TypeName = "datetime")]
		public DateTime StartTime { get; set; }
		/// <summary>
		/// 结束时间
		/// </summary>
		[Column(TypeName = "datetime")]
		public DateTime EndTime { get; set; }

		/// <summary>
		/// 密码 密码打卡用 这里只是用作打卡的数字验证 不用加密
		/// </summary>
		[MaxLength(10)]
		public string? PassWord { get; set; } = "";

		// 二维码
		[MaxLength(100)]
		public string? QRCode { get; set; } = "";



		[ForeignKey("Course")]
		public int CourseId { get; set; }
		public virtual Course Course { get; set; }

		public virtual List<Attendance> Attendances { get; set; } // 考勤
	}
}
