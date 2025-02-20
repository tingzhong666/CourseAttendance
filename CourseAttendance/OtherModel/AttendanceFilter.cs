namespace CourseAttendance.OtherModel
{
	public class AttendanceFilter
	{
		public int? CourseId { get; set; }
		public string? StudentId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
