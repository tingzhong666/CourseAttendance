using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CourseTimeExt
	{
		public static CourseTimeResDto ToDto(this CourseTime model)
		{
			return new CourseTimeResDto
			{
				CourseId = model.CourseId,
				//EndTime = model.EndTime,
				//StartTime = model.StartTime,
				TimeTableId = model.TimeTableId,
				//Weekday = model.Weekday,
				 DateDay = model.DateDay,
			};
		}
	}
}
