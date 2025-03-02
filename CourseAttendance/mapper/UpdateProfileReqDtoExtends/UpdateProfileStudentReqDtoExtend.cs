using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UpdateProfileReqDtoExtends
{
	public static class UpdateProfileStudentReqDtoExtend
	{
		public static Student ToStudentModel(this UpdateProfileStudentReqDto dto)
		{
			return new Student
			{
				GradeId = dto.GradeId,
			};
		}
	}
}
