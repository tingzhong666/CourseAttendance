using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper.UserExts
{
	public static class CourseExt
	{
		public static CourseRequestDto? ToRequestDto(this Course model)
		{
			if (model == null) return null;

			return new CourseRequestDto
			{
				Name = model.Name,
				Weekday = model.Weekday,
				StartTime = model.StartTime,
				EndTime = model.EndTime,
				Location = model.Location,
				TeacherId = model.TeacherId
			};
		}


		// 将 Course 转换为 CourseResponseDto
		public static CourseResponseDto? ToResponseDto(this Course model)
		{
			if (model == null) return null;

			return new CourseResponseDto
			{
				Id = model.Id,
				Name = model.Name,
				Weekday = model.Weekday,
				StartTime = model.StartTime,
				EndTime = model.EndTime,
				Location = model.Location,
				CreatedAt = model.CreatedAt,
				UpdatedAt = model.UpdatedAt,
				TeacherId = model.TeacherId
				// 这里可以添加教师信息的转换
			};
		}
	}
}
