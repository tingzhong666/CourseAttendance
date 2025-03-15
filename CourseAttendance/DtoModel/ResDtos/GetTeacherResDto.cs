namespace CourseAttendance.DtoModel.ResDtos
{
	public class GetTeacherResDto : GetUserResDto
	{
		public List<int> CourseIds { get; set; }
	}
}
