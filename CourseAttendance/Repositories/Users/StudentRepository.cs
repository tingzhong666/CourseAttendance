using CourseAttendance.AppDataContext;
using CourseAttendance.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories.Users
{
	public class StudentRepository
	{
		private readonly AppDBContext _context;

		public StudentRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Student?> GetByIdAsync(string userId)
		{
			return await _context.Students
				.Include(s => s.User)
				.Include(s => s.Grade)
				.Include(s => s.CourseStudents)
				.Include(s => s.Attendances)
				.FirstOrDefaultAsync(s => s.UserId == userId);
		}

		public async Task<IEnumerable<Student>> GetAllAsync()
		{
			return await _context.Students
				.Include(s => s.User)
				.Include(s => s.Grade)
				.Include(s => s.CourseStudents)
				.Include(s => s.Attendances)
				.ToListAsync();
		}

		public async Task AddAsync(Student student)
		{
			var model = new Student
			{
				UserId = student.UserId,
				Grade = student.Grade
			};
			await _context.Students.AddAsync(model);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Student student)
		{
			var model = await _context.Students.FirstAsync(s => s.UserId == student.UserId);
			if (model == null) return;

			model.GradeId = student.GradeId;
			_context.Students.Update(model);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string userId)
		{
			var student = await GetByIdAsync(userId);
			if (student != null)
			{
				_context.Students.Remove(student);
				await _context.SaveChangesAsync();
			}
		}
	}
}
