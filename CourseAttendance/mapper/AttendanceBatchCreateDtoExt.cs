using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class AttendanceBatchCreateDtoExt
	{
		public static AttendanceBatch ToModel(this AttendanceBatchCreateDto dto)
		{
			var now = DateTime.Now;
			return new AttendanceBatch
			{
				PassWord = dto.PassWord,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				CourseId = dto.CourseId,
				CheckMethod = dto.CheckMethod,
				CreatedAt = now,
				UpdatedAt = now,
			};
		}
	}
}
