﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.Model
{
	/// <summary>
	/// 作息表
	/// </summary>
	public class TimeTable
	{
		[Key]
		public int Id { get; set; }
		/// <summary>
		/// 描述名称
		/// </summary>
		[Required]
		[MaxLength(10)]
		public string Name { get; set; }
		[Required]
		public TimeSpan Start { get; set; }
		[Required]
		public TimeSpan End { get; set; }


		[Required]
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public virtual List<CourseTime> CourseTimes { get; set; }

}
}
