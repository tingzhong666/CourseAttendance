﻿using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class UpdateProfileReqDto
	{
		//public required string Id;
		/// <summary>
		/// 邮件
		/// </summary>
		public string? Email { get; set; }
		/// <summary>
		/// 手机号
		/// </summary>
		public string? Phone { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		[Required]
		public required string Name { get; set; }
	}
}
