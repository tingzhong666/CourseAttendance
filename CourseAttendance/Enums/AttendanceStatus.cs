using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 考勤状态
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum AttendanceStatus
	{
		///// <summary>
		///// 迟到
		///// </summary>
		//Late,
		///// <summary>
		///// 早退
		///// </summary>
		//Early,
		/// <summary>
		/// 请假
		/// </summary>
		Leave,
		/// <summary>
		/// 缺席
		/// </summary>
		Absent,
		/// <summary>
		/// 正常
		/// </summary>
		None
	}
}
