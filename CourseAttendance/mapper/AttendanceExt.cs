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
				//AttendanceDate = attendance.AttendanceDate,
				SignInTime = attendance.SignInTime,
				//SignOutTime = attendance.SignOutTime,
				Status = attendance.Status,
				Remark = attendance.Remark,
				CheckMethod = attendance.CheckMethod,
				//Location = attendance.Location,
				CreatedAt = attendance.CreatedAt,
				UpdatedAt = attendance.UpdatedAt,
				//AttachmentUrl = attendance.AttachmentUrl,
				CourseId = attendance.CourseId,
				StudentId = attendance.StudentId,
			};
		}
	}
}
