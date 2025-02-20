using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using CourseAttendance.OtherModel;
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

		public async Task<IEnumerable<Attendance>> GetAllAsync()
		{
			return await _context.Attendances
				.Include(a => a.Course)
				.Include(a => a.Student)
				.ToListAsync();
		}

		public async Task AddAsync(Attendance attendance)
		{
			await _context.Attendances.AddAsync(attendance);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Attendance attendance)
		{
			_context.Attendances.Update(attendance);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int courseId, string studentId)
		{
			var attendance = await GetByIdAsync(courseId, studentId);
			if (attendance != null)
			{
				_context.Attendances.Remove(attendance);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<IEnumerable<Attendance>> FilterAttendancesAsync(AttendanceFilter filter)
		{
			var query = _context.Attendances.AsQueryable();

			if (filter.CourseId.HasValue)
			{
				query = query.Where(a => a.CourseId == filter.CourseId.Value);
			}

			if (!string.IsNullOrEmpty(filter.StudentId))
			{
				query = query.Where(a => a.StudentId == filter.StudentId);
			}

			if (filter.StartDate.HasValue)
			{
				query = query.Where(a => a.AttendanceDate >= filter.StartDate.Value);
			}

			if (filter.EndDate.HasValue)
			{
				query = query.Where(a => a.AttendanceDate <= filter.EndDate.Value);
			}

			return await query.ToListAsync();
		}
	}
}
