namespace CourseAttendance.DtoModel.ResDtos
{
	public class GradeResponseDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		//public List<GetStudentResDto> Students { get; set; } // 学生信
	}
}
