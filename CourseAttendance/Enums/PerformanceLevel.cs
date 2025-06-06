﻿using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 上课表现
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum PerformanceLevel
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,
		/// <summary>
		/// 优
		/// </summary>
		Excellent,
		/// <summary>
		/// 良
		/// </summary>
		Good,
		/// <summary>
		/// 中
		/// </summary>
		Medium,
		/// <summary>
		/// 差
		/// </summary>
		Poor
	}
}
