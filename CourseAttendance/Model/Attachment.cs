namespace CourseAttendance.Model
{
	public class Attachment
	{
		public int Id { get; set; }
		public int AttendanceId { get; set; }
		public string Url { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public virtual Attendance Attendance { get; set; }
	}
}
