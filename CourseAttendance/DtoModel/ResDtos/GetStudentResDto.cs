namespace CourseAttendance.DtoModel.ResDtos
{
	public class GetStudentResDto : GetUserResDto
	{
		/// <summary>
		/// 班级
		/// </summary>
		public int GradeId { get; set; }
	}
}
