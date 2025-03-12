using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class GradeRepository
	{
		private readonly AppDBContext _context;

		public GradeRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Grade?> GetByIdAsync(int id)
		{
			return await _context.Grades
				.Include(g => g.Students)
				.ThenInclude(s => s.User)
				.FirstOrDefaultAsync(g => g.Id == id);
		}

		public async Task<IEnumerable<Grade>> GetAllAsync()
		{
			return await _context.Grades
				.Include(g => g.Students)
				.ToListAsync();
		}

		public async Task<int> AddAsync(Grade grade)
		{
			await _context.Grades.AddAsync(grade);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> UpdateAsync(Grade grade)
		{
			var model = await GetByIdAsync(grade.Id);
			if (model == null) return 0;

			model.Name = grade.Name;
			model.UpdatedAt = DateTime.Now;

			_context.Grades.Update(model);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int id)
		{
			var grade = await GetByIdAsync(id);
			if (grade != null)
			{
				_context.Grades.Remove(grade);
				return await _context.SaveChangesAsync();
			}
			return 0;
		}
	}
}
