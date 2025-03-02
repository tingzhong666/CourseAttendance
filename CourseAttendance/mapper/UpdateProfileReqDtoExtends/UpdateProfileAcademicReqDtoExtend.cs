using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UpdateProfileReqDtoExtends
{
	public static class UpdateProfileAcademicReqDtoExtend
	{
		public static Academic ToAcademicModel(this UpdateProfileAcademicReqDto dto)
		{
			return new Academic
			{
			};
		}
	}
}
