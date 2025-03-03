using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CreateUserAdminReqDtoExt
	{
		public static Admin ToModel(this CreateUserAdminReqDto dto)
		{
			return new Admin
			{
			};
		}
	}
}
