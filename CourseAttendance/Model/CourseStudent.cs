namespace CourseAttendance.Model
{
	public class CourseStudent
	{
		public int CourseId { get; set; }
		public int StudentId { get; set; }

		public virtual Course Course { get; set; }
		public virtual User Student { get; set; }
	}
}
