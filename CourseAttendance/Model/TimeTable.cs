using System.ComponentModel.DataAnnotations;

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
		public string Name { get; set; }
		[Required]
		public TimeSpan Start { get; set; }
		[Required]
		public TimeSpan End { get; set; }


		public virtual List<CourseTime> CourseTimes { get; set; }

}
}
