namespace CourseAttendance.DtoModel.ResDtos
{
	public class CourseResponseListDto
	{
		public List<CourseResponseDto> DataList { get; set; } = new List<CourseResponseDto>();
		public int Total { get; set; } = 0;
	}
}
