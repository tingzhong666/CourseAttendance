using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class CourseRepository
	{
		private readonly AppDBContext _context;

		public CourseRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<Course?> GetByIdAsync(int id)
		{
			return await _context.Courses
				.Include(c => c.Teacher)
				.Include(c => c.CourseStudents)
				.Include(c => c.Attendances)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<IEnumerable<Course>> GetAllAsync()
		{
			return await _context.Courses
				.Include(c => c.Teacher)
				.Include(c => c.CourseStudents)
				.Include(c => c.Attendances)
				.ToListAsync();
		}

		public async Task<int> AddAsync(Course course)
		{
			await _context.Courses.AddAsync(course);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> UpdateAsync(Course course)
		{

			var model = await GetByIdAsync(course.Id);
			if (model == null) return 0;

			model.Name = course.Name;
			model.Weekday = course.Weekday;
			model.StartTime = course.StartTime;
			model.EndTime = course.EndTime;
			model.Location = course.Location;
			model.UpdatedAt = DateTime.Now;
			model.TeacherUserId = course.TeacherUserId;

			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int id)
		{
			var course = await GetByIdAsync(id);
			if (course != null)
			{
				_context.Courses.Remove(course);
				return await _context.SaveChangesAsync();
			}

			return 0;
		}
	}
}
