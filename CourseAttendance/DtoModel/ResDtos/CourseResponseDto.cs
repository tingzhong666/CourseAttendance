namespace CourseAttendance.DtoModel.ResDtos
{
	public class CourseResponseDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Weekday { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Location { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string TeacherId { get; set; }

		//public UserResponseDto Teacher { get; set; } // 如果需要提供教师信息
	}
}
