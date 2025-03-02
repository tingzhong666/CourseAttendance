namespace CourseAttendance.DtoModel.ResDtos
{
	public class GetUserResDto
	{
		public required string Id { get; set; }
		public required string Name { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		public required string UserName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
