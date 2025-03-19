using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class AttendanceRepository
	{
		private readonly AppDBContext _context;

		public AttendanceRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Attendance?> GetByIdAsync(int courseId, string studentId)
		{
			return await _context.Attendances
				.Include(a => a.Course)
				.Include(a => a.Student)
				.FirstOrDefaultAsync(a => a.CourseId == courseId && a.StudentId == studentId);
		}

		public async Task<List<Attendance>> GetAllAsync()
		{
			return await _context.Attendances
				.Include(a => a.Course)
				.Include(a => a.Student)
				.ToListAsync();
		}

		public async Task<int> AddAsync(Attendance attendance)
		{
			await _context.Attendances.AddAsync(attendance);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> UpdateAsync(Attendance attendance)
		{
			var model = await _context.Attendances
				.Include(a => a.Course)
				.Include(a => a.Student)
				.FirstOrDefaultAsync(a => a.CourseId == attendance.CourseId && a.StudentId == attendance.StudentId);
			if (model == null) return 0;

			model.SignInTime = attendance.SignInTime;
			//model.SignOutTime = attendance.SignOutTime;
			model.Status = attendance.Status;
			//model.Performance = attendance.Performance;
			model.Remark = attendance.Remark;
			model.CheckMethod = attendance.CheckMethod;
			//model.Location = attendance.Location;
			model.UpdatedAt = DateTime.Now;
			model.AttachmentUrl = attendance.AttachmentUrl;
			model.PassWord = attendance.PassWord;
			model.EndTime = attendance.EndTime;

			//_context.Attendances.Update(model);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int courseId, string studentId)
		{
			var attendance = await GetByIdAsync(courseId, studentId);
			if (attendance != null)
			{
				_context.Attendances.Remove(attendance);
				return await _context.SaveChangesAsync();
			}
			return 0;
		}


		//public async Task<IEnumerable<Attendance>> FilterAttendancesAsync(AttendanceFilter filter)
		//{
		//	var query = _context.Attendances.AsQueryable();

		//	if (filter.CourseId.HasValue)
		//	{
		//		query = query.Where(a => a.CourseId == filter.CourseId.Value);
		//	}

		//	if (!string.IsNullOrEmpty(filter.StudentId))
		//	{
		//		query = query.Where(a => a.StudentId == filter.StudentId);
		//	}

		//	if (filter.StartDate.HasValue)
		//	{
		//		query = query.Where(a => a.CreatedAt >= filter.StartDate.Value);
		//	}

		//	if (filter.EndDate.HasValue)
		//	{
		//		query = query.Where(a => a.CreatedAt <= filter.EndDate.Value);
		//	}

		//	return await query.ToListAsync();
		//}
	}
}
