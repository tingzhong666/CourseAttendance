using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class CourseSelectionReqDtoExt
	{
		public static CourseStudent ToModel(this CourseSelectionReqDto dto)
		{
			return new CourseStudent
			{
				Performance = dto.Performance,
			};
		}
	}
}
