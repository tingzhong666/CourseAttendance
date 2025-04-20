using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
				.Include(c => c.MajorsSubcategory)
				.Include(c => c.CourseStudents)
				.Include(c => c.AttendanceBatchs)
				.Include(c => c.CourseTimes)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<(List<Course> data, int total)> GetAllAsync(CourseReqQueryDto query)
		{
			var coursesQ = _context.Courses.AsQueryable();
			if (query.q != null && query.q != "")
			{
				coursesQ = coursesQ.Where(x => x.Name.Contains(query.q));
			}
			if (query.studentIds.Count != 0)
			{
				coursesQ = coursesQ.Where(x => query.studentIds.Any(v => x.CourseStudents.Any(y => y.StudentId == v)));
			}
			if (query.TeacherIds.Count != 0)
			{
				coursesQ = coursesQ.Where(x => query.TeacherIds.Contains(x.TeacherUserId));
			}

			// 大专业
			if (query.MajorsCategoryId != null)
			{
				coursesQ = coursesQ.Where(x => x.MajorsSubcategory.MajorsCategoriesId == query.MajorsCategoryId);
			}
			// 小专业
			if (query.MajorsSubcategoriesId != null)
			{
				coursesQ = coursesQ.Where(x => x.MajorsSubcategoryId == query.MajorsSubcategoriesId);
			}


			// 创建时间排序
			if (query.SortCreateTime != null && query.SortCreateTime == 1) // 降
			{
				coursesQ = coursesQ.OrderByDescending(x => x.CreatedAt);
			}
			if (query.SortCreateTime != null && query.SortCreateTime == 0) // 升
			{
				coursesQ = coursesQ.OrderBy(x => x.CreatedAt);
			}

			var courses = await coursesQ
				.Include(c => c.Teacher)
				.Include(c => c.MajorsSubcategory)
				.Include(c => c.CourseStudents)
				.Include(c => c.AttendanceBatchs)
				.Include(c => c.CourseTimes)
				.ToListAsync();

			var total = courses.Count;
			// 分页
			courses = courses.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (data: courses, total);
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
			model.Location = course.Location;
			model.UpdatedAt = DateTime.Now;
			model.TeacherUserId = course.TeacherUserId;
			model.MajorsSubcategoryId = course.MajorsSubcategoryId;

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
