namespace CourseAttendance.DtoModel.ReqDtos
{
	public class ReqQueryDto
	{
		public int Page { get; set; } = 1;
		public int Limit { get; set; } = 10;
		public string? q { get; set; } = "";
	}
}
