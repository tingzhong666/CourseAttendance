using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class AcademicRepository
	{
		private readonly AppDBContext _context;

		public AcademicRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Academic?> GetByIdAsync(string userId)
		{
			return await _context.Academics
				.Include(a => a.User)
				.FirstOrDefaultAsync(a => a.UserId == userId);
		}

		public async Task<IEnumerable<Academic>> GetAllAsync()
		{
			return await _context.Academics
				.Include(a => a.User)
				.ToListAsync();
		}

		public async Task<int> AddAsync(Academic academic)
		{
			await _context.Academics.AddAsync(academic);
			var res = await _context.SaveChangesAsync();
			return res;
		}

		public async Task UpdateAsync(Academic academic)
		{
			_context.Academics.Update(academic);
			await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(string userId)
		{
			var academic = await GetByIdAsync(userId);
			if (academic != null)
			{
				_context.Academics.Remove(academic);
				return await _context.SaveChangesAsync();
			}
			return 0;
		}
	}
}
