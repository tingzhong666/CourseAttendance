using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CreateUserStudentReqDtoExt
	{
		public static Student ToModel(this CreateUserStudentReqDto dto)
		{
			return new Student
			{
				GradeId = dto.GradId
			};
		}
	}
}
