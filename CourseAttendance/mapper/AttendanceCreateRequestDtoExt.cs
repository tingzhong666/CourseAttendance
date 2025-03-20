using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Enums;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class AttendanceCreateRequestDtoExt
	{
		public static Attendance ToModel(this AttendanceCreateRequestDto dto, string studentId)
		{
			var now = DateTime.Now;
			return new Attendance
			{
				CheckMethod = dto.CheckMethod,
				CourseId = dto.CourseId,
				CreatedAt = now,
				Status = AttendanceStatus.Absent,
				UpdatedAt = now,
				StudentId = studentId,
				EndTime = dto.EndTime,
				PassWord = dto.PassWord,
				Remark = "",
			};
		}
	}
}
