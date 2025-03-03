using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CreateUserAcademicReqDtoExt
	{
		public static Academic ToModel(this CreateUserAcademicReqDto dto)
		{
			return new Academic
			{
			};
		}
	}
}
