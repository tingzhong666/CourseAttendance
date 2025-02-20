using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
    public class CourseStudent
	{
		public int CourseId { get; set; }
		public string StudentId { get; set; }

		public virtual Course Course { get; set; }
		public virtual Student Student { get; set; }
	}
}
