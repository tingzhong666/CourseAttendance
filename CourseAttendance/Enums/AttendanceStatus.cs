using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 考勤状态
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum AttendanceStatus
	{
		/// <summary>
		/// 未处理
		/// </summary>
		None,
		/// <summary>
		/// 正常签到
		/// </summary>
		Ok,
		/// <summary>
		/// 老师手动修改签到
		/// </summary>
		OkTearcher,
		/// <summary>
		/// 请假
		/// </summary>
		Leave,
		/// <summary>
		/// 缺席
		/// </summary>
		Absent,
	}
}
