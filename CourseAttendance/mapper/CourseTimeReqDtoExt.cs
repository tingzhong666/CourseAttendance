using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class CourseTimeReqDtoExt
	{
		public static CourseTime ToModel(this CourseTimeReqDto dto)
		{
			return new CourseTime
			{
				CourseId = dto.CourseId,
				TimeTableId = dto.TimeTableId,
				//EndTime = dto.EndTime,
				//StartTime = dto.StartTime,
				//Weekday = dto.Weekday,
				DateDay = dto.DateDay,
			};
		}
	}
}
