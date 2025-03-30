using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 签到方式
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum CheckMethod
	{
		/// <summary>
		/// 普通打卡
		/// </summary>
		Normal,
		/// <summary>
		/// 位置打卡
		/// </summary>
		//Location,
		/// <summary>
		/// 密码打卡
		/// </summary>
		Password
	}
}
