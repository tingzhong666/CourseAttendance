using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class AttendanceExt
	{
		public static AttendanceResponseDto ToDto(this Attendance attendance)
		{
			return new AttendanceResponseDto
			{
				CreatedAt = attendance.CreatedAt,
				UpdatedAt = attendance.UpdatedAt,

				Id = attendance.Id,
				CheckMethod = attendance.CheckMethod,
				EndTime = attendance.EndTime,
				StartTime = attendance.StartTime,
				Status = attendance.Status,
				SignInTime = attendance.SignInTime,
				Remark = attendance.Remark,

				CourseId = attendance.CourseId,
				StudentId = attendance.StudentId,
			};
		}
	}
}
