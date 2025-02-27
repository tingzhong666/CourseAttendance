namespace CourseAttendance.DtoModel.ReqDtos
{
	public class UpdateProfileStudentReqDto: UpdateProfileReqDto
	{
		/// <summary>
		/// 班级
		/// </summary>
		public int GradeId { get; set; } 
	}
}
