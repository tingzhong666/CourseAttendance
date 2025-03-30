using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class TimeTableExt
	{
		public static TimeTableResDto ToDto(this TimeTable model)
		{
			return new TimeTableResDto
			{
				End = model.End,
				Id = model.Id,
				Name = model.Name,
				Start = model.Start,
			};
		}
	}
}
