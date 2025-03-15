using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class TeacherRepository
	{
		private readonly AppDBContext _context;

		public TeacherRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Teacher?> GetByIdAsync(string userId)
		{
			return await _context.Teachers
				.Include(t => t.User)
				.Include(t => t.Courses)
				.FirstOrDefaultAsync(t => t.UserId == userId);
		}

		public async Task<IEnumerable<Teacher>> GetAllAsync()
		{
			return await _context.Teachers
				.Include(t => t.User)
				.ToListAsync();
		}

		public async Task<int> AddAsync(Teacher teacher)
		{
			await _context.Teachers.AddAsync(teacher);
			return await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Teacher teacher)
		{
			_context.Teachers.Update(teacher);
			await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(string userId)
		{
			var teacher = await GetByIdAsync(userId);
			if (teacher != null)
			{
				_context.Teachers.Remove(teacher);
				return await _context.SaveChangesAsync();
			}
			return 0;
		}
	}
}
