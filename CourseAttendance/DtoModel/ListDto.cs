namespace CourseAttendance.DtoModel
{
	public class ListDto<T>
	{
		public List<T> DataList { get; set; } = new List<T>();
		public int Total { get; set; } = 0;
	}
}
