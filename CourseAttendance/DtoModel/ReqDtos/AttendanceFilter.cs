namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceFilter
	{
		public int? CourseId { get; set; }
		public string? StudentId { get; set; }
		/// <summary>
		/// 考勤创建的时间范围 起始
		/// </summary>
		public DateTime? StartDate { get; set; }
		/// <summary>
		/// 考勤创建的时间范围 终末
		/// </summary>
		public DateTime? EndDate { get; set; }
	}
}
