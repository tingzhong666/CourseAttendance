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
		/// 密码打卡
		/// </summary>
		Password,
		/// <summary>
		/// 二维码打卡
		/// </summary>
		TowCode,
		/// <summary>
		/// 拍照打卡
		/// </summary>
		//Photos,
		/// <summary>
		/// 位置打卡
		/// </summary>
		//Location,
	}
}
