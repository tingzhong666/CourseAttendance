﻿using System.Text.Json.Serialization;

namespace CourseAttendance.Enums
{
	/// <summary>
	/// 用户身份 权限
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UserRole
	{
		/// <summary>
		/// 系统超管
		/// </summary>
		Admin,
		/// <summary>
		/// 教务处
		/// </summary>
		Academic,
		/// <summary>
		/// 授课老师
		/// </summary>
		Teacher,
		/// <summary>
		/// 学生
		/// </summary>
		Student
	}
}
