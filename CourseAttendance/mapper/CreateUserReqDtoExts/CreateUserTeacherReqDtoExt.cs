using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CreateUserTeacherReqDtoExt
	{
		public static Teacher ToModel(this CreateUserTeacherReqDto dto)
		{
			return new Teacher
			{
			};
		}
	}
}
