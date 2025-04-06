using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AccountReqQueryDto: ReqQueryDto
	{
		public List<UserRole>? Roles { get; set; } = [];
	}
}
