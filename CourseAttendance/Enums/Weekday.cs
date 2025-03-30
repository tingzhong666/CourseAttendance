using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 周几
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Weekday
	{
		/// <summary>
		/// 周一
		/// </summary>
		MONDAY,
		/// <summary>
		/// 周二
		/// </summary>
		TUESDAY,
		/// <summary>
		/// 周三
		/// </summary>
		WEDNESDAY,
		/// <summary>
		/// 周四
		/// </summary>
		THURSDAY,
		/// <summary>
		/// 周五
		/// </summary>
		FRIDAY,
		/// <summary>
		/// 周六
		/// </summary>
		SATURDAY,
		/// <summary>
		/// 周日
		/// </summary>
		SUNDAY
	}
}
