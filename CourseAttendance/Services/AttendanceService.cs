using CourseAttendance.AppDataContext;
using CourseAttendance.Enums;
using CourseAttendance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Services
{
	public class AttendanceService
	{
		private readonly AttendanceRepository _attendanceRepository;
		private readonly AppDBContext _context;

		public AttendanceService(AttendanceRepository attendanceRepository, AppDBContext context)
		{
			_attendanceRepository = attendanceRepository;
			_context = context;
		}

		// 超时未处理 缺席检测
		public async Task UpdateAbsentStatusAsync()
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var now = DateTime.Now;
				// 获取当天未处理的考勤数据
				var models = await _attendanceRepository.GetAllAsync(new DtoModel.ReqDtos.AttendanceReqQueryDto
				{
					StartTime = DateTime.Today,
					EndTime = now,
					AttendanceStatus = AttendanceStatus.None,
				});

				// 筛选超时
				var models2 = models.queryRes.Where(x => x.AttendanceBatch.EndTime < now).ToList();

				// 设置缺席
				foreach (var x in models2)
				{
					x.Status = AttendanceStatus.Absent;
					await _attendanceRepository.UpdateAsync(x);
				}
				await transaction.CommitAsync();
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
			}
		}
	}
}
