using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class GradeRequestDtoExt
	{


		public static Grade ToModel(this GradeRequestDto dto)
		{
			if (dto == null) return null;

			return new Grade
			{
				Name = dto.Name
			};
		}
	}
}
