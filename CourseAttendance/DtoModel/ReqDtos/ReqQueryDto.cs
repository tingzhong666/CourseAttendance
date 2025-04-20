namespace CourseAttendance.DtoModel.ReqDtos
{
	public class ReqQueryDto
	{
		public int Page { get; set; } = 1;
		public int Limit { get; set; } = 10;
		public string? q { get; set; } = "";

		// 排序 创建时间 0升序 1降序
		public int? SortCreateTime { get; set; } = 1;
	}
}
