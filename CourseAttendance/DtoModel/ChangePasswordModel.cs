namespace CourseAttendance.DtoModel
{
	public class ChangePasswordModel
	{
		public string NewPassword { get; internal set; }
		public string ConfirmPassword { get; internal set; }
		public string CurrentPassword { get; internal set; }
	}
}