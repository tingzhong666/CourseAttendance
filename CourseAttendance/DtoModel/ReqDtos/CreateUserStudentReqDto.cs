namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CreateUserStudentReqDto : CreateUserReqDto
	{
		/// <summary>
		/// 班级ID
		/// </summary>
		public required int GradId;
	}
}
