using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class AttendanceRequestDtoExt
	{
		public static Attendance ToModel(this AttendanceRequestDto requestDto)
		{
			return new Attendance
			{
				AttendanceDate = requestDto.AttendanceDate,
				SignInTime = requestDto.SignInTime,
				//SignOutTime = requestDto.SignOutTime,
				Status = requestDto.Status,
				Remark = requestDto.Remark,
				CheckMethod = requestDto.CheckMethod,
				Location = requestDto.Location,
				AttachmentUrl = requestDto.AttachmentUrl,
				CourseId = requestDto.CourseId,
				StudentId = requestDto.StudentId,
				//CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
		}
	}
}
