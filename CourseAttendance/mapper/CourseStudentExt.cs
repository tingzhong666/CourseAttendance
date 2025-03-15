using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class CourseStudentExt
	{
		public static CourseSelectionResDto ToDto(this CourseStudent model)
		{
			return new CourseSelectionResDto
			{
				Performance = model.Performance,
				CourseId = model.CourseId,
				StudentId = model.StudentId,
			};
		}
	}
}
