using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UpdateProfileReqDtoExtends
{
	public static class UpdateProfileAdminReqDtoExtend
	{
		public static Admin ToAdminModel(this UpdateProfileAdminReqDto dto)
		{
			return new Admin
			{
			};
		}
	}
}
