using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class AdminRepository
	{
		private readonly AppDBContext _context;

		public AdminRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Admin?> GetAdminByIdAsync(string userId)
		{
			return await _context.Admins.Include(a => a.User)
										 .FirstOrDefaultAsync(a => a.UserId == userId);
		}

		public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
		{
			return await _context.Admins.Include(a => a.User).ToListAsync();
		}

		public async Task AddAdminAsync(Admin admin)
		{
			await _context.Admins.AddAsync(admin);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAdminAsync(Admin admin)
		{
			_context.Admins.Update(admin);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAdminAsync(int userId)
		{
			var admin = await _context.Admins.FindAsync(userId);
			if (admin != null)
			{
				_context.Admins.Remove(admin);
				await _context.SaveChangesAsync();
			}
		}
	}
}
