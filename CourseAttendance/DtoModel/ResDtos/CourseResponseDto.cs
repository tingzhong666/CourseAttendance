﻿using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class CourseResponseDto
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Weekday { get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		[Required]
		public DateTime EndTime { get; set; }
		[Required]
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		[Required]
		public string TeacherId { get; set; }

		//public UserResponseDto Teacher { get; set; } // 如果需要提供教师信息
		[Required]
		public List<string> StudentIds { get; set; } = [];
	}
}
