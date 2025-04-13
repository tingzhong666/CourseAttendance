namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceReqQueryDto : ReqQueryDto
	{
		/// <summary>
		/// 学生id
		/// </summary>
		public List<string>? StudentId { get; set; }
		/// <summary>
		/// 学生名 如果有学生id 优先学生id
		/// </summary>
		public string? StudentName { get; set; }
		/// <summary>
		/// 老师id
		/// </summary>
		public List<string>? TeacherId { get; set; }
		/// <summary>
		/// 老师名 如果有老师id 优先老师id
		/// </summary>
		public string? TeacherName { get; set; }
		/// <summary>
		/// 时间范围
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 时间范围
		/// </summary>
		public DateTime? EndTime { get; set; }
	}
}
