using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UpdateProfileReqDtoExtends
{
	public static class UpdateProfileTeacherReqDtoExtend
	{
		public static Teacher ToTeacherModel(this UpdateProfileTeacherReqDto dto)
		{
			return new Teacher
			{
			};
		}
	}
}
