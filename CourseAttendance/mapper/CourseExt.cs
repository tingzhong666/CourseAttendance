using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class CourseExt
	{
		//public static CourseRequestDto? ToRequestDto(this Course model)
		//{
		//	if (model == null) return null;

		//	return new CourseRequestDto
		//	{
		//		Name = model.Name,
		//		Weekday = model.Weekday,
		//		StartTime = model.StartTime,
		//		EndTime = model.EndTime,
		//		Location = model.Location,
		//		TeacherId = model.TeacherUserId
		//	};
		//}


		// 将 Course 转换为 CourseResponseDto
		public static CourseResponseDto? ToResponseDto(this Course model)
		{
			return new CourseResponseDto
			{
				Id = model.Id,
				Name = model.Name,
				//Weekday = model.Weekday,
				//StartTime = model.StartTime,
				//EndTime = model.EndTime,
				CourseTimes = model.CourseTimes.Select(x => x.ToDto()).ToList(),
				Location = model.Location,
				CreatedAt = model.CreatedAt,
				UpdatedAt = model.UpdatedAt,
				TeacherId = model.TeacherUserId,
				StudentIds = model.CourseStudents.Select(x => x.StudentId).ToList(),
				MajorsSubcategoryId = model.MajorsSubcategoryId,
			};
		}
	}
}
