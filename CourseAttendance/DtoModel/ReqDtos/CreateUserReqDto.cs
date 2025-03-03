namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CreateUserReqDto
	{
		/// <summary>
		/// 邮件
		/// </summary>
		public string? Email;
		/// <summary>
		/// 手机号
		/// </summary>
		public string? Phone;
		/// <summary>
		/// 姓名
		/// </summary>
		public required string Name;
		/// <summary>
		/// 工号
		/// </summary>
		public required string UserName;
		/// <summary>
		/// 密码
		/// </summary>
		public required string PassWord;
	}
}
