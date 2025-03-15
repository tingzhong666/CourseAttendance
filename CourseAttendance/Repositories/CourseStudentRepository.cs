using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class CourseStudentRepository
	{
		private readonly AppDBContext _context;

		public CourseStudentRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<CourseStudent?> GetByIdsAsync(int courseId, string studentId)
		{
			return await _context.CourseStudents
				.Include(cs => cs.Course)
				.Include(cs => cs.Student)
				.FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);
		}

		public async Task<IEnumerable<CourseStudent>> GetAllAsync()
		{
			return await _context.CourseStudents
				.Include(cs => cs.Course)
				.Include(cs => cs.Student)
				.ToListAsync();
		}

		public async Task<int> AddAsync(CourseStudent courseStudent)
		{
			await _context.CourseStudents.AddAsync(courseStudent);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int courseId, string studentId)
		{
			var courseStudent = await GetByIdsAsync(courseId, studentId);
			if (courseStudent != null)
			{
				_context.CourseStudents.Remove(courseStudent);
				return await _context.SaveChangesAsync();
			}

			return 0;
		}

		// 修改
		public async Task<int> UpdateAsync(CourseStudent courseStudent)
		{
			var model = await _context.CourseStudents.FirstOrDefaultAsync(x => x.StudentId == courseStudent.StudentId && x.CourseId == courseStudent.CourseId);
			if (model == null) return 0;
			model.Performance = courseStudent.Performance;
			return await _context.SaveChangesAsync();
		}
	}
}
