using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class AttendanceBatchExt
	{
		public static AttendanceBatchResDto ToDto(this AttendanceBatch model)
		{
			return new AttendanceBatchResDto
			{
				CreatedAt = model.CreatedAt,
				UpdatedAt = model.UpdatedAt,
				Id = model.Id,
				CheckMethod = model.CheckMethod,
				StartTime = model.StartTime,
				EndTime = model.EndTime,
				CourseId = model.CourseId,
				AttendanceIds = model.Attendances.Select(x => x.Id).ToList(),

				PassWord = model.PassWord,
				QRCode = model.QRCode,
			};
		}
	}
}
