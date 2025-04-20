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
				CheckMethod = attendance.AttendanceBatch.CheckMethod,
				EndTime = attendance.AttendanceBatch.EndTime,
				StartTime = attendance.AttendanceBatch.StartTime,
				Status = attendance.Status,
				SignInTime = attendance.SignInTime,
				Remark = attendance.Remark,

				CourseId = attendance.AttendanceBatch.CourseId,
				StudentId = attendance.StudentId,
				AttendanceBatchId = attendance.AttendanceBatch.Id,
			};
		}
	}
}
