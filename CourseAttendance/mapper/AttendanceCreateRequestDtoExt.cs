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
				CreatedAt = now,
				UpdatedAt = now,
				//CheckMethod = dto.CheckMethod,
				//StartTime = dto.StartTime,
				//EndTime = dto.EndTime,
				Status = AttendanceStatus.None,
				//SignInTime = dto.SignInTime,
				Remark = "",
				//PassWord = dto.PassWord,
				//CourseId = dto.CourseId,
				StudentId = studentId,
			};
		}
	}
}
