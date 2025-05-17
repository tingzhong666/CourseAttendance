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
		None = 0,
		/// <summary>
		/// 正常签到
		/// </summary>
		Ok = 1,
		/// <summary>
		/// 老师手动修改签到
		/// </summary>
		OkTearcher = 2,
		/// <summary>
		/// 请假
		/// </summary>
		Leave = 3,
		/// <summary>
		/// 缺席
		/// </summary>
		Absent = 4,
	}
}
