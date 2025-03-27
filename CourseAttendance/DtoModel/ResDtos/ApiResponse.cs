namespace CourseAttendance.DtoModel.ResDtos
{
	public class ApiResponse<T>
	{
		public int Code { get; set; } = 1;
		public string? Msg { get; set; } = "";
		public T Data { get; set; }
	}
}
