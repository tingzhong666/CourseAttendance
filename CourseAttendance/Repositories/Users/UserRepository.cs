using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class UserRepository
	{
		private readonly AppDBContext _context;

		public UserRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<User?> GetByIdAsync(string id)
		{
			return await _context.Users
				.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task AddAsync(User user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			var user = await GetByIdAsync(id);
			if (user != null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}
	}
}
