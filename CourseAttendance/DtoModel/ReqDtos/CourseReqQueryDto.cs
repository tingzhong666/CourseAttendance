namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseReqQueryDto
	{
		public int Page { get; set; } = 1;
		public int Limit { get; set; } = 10;
		public string? Name { get; set; } = "";
	}
}