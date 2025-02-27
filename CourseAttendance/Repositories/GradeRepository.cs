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
				.FirstOrDefaultAsync(g => g.Id == id);
		}

		public async Task<IEnumerable<Grade>> GetAllAsync()
		{
			return await _context.Grades
				.Include(g => g.Students)
				.ToListAsync();
		}

		public async Task AddAsync(Grade grade)
		{
			await _context.Grades.AddAsync(grade);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Grade grade)
		{
			var model = await GetByIdAsync(grade.Id);
			if (model == null) return;

			model.Name = grade.Name;
			model.UpdatedAt = DateTime.Now;

			_context.Grades.Update(model);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var grade = await GetByIdAsync(id);
			if (grade != null)
			{
				_context.Grades.Remove(grade);
				await _context.SaveChangesAsync();
			}
		}
	}
}
