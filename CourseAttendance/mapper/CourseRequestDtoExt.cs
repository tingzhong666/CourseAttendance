using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class CourseRequestDtoExt
	{
		public static Course ToModel(this CourseRequestDto dto)
		{
			return new Course
			{
				Name = dto.Name,
				//Weekday = dto.Weekday,
				//StartTime = dto.StartTime,
				//EndTime = dto.EndTime,
				Location = dto.Location,
				TeacherUserId = dto.TeacherId,
				//CourseTimes = dto.CourseTimes.Select(x => x.ToModel()).ToList()
			};
		}
	}
}
