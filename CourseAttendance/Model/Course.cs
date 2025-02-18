﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model
{
	public class Course
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int TeacherId { get; set; }
		public byte Weekday { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		public virtual User Teacher { get; set; }
	}
}
