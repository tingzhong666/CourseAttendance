using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class CourseTimeRepository
	{
		private readonly AppDBContext _context;

		public CourseTimeRepository(AppDBContext context)
		{
			_context = context;
		}

		public async Task<CourseTime?> GetByIdAsync(int courseId, int timeTableId)
		{
			return await _context.CourseTimes
				.Include(cs => cs.Course)
				.Include(cs => cs.TimeTable)
				.FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.TimeTableId == timeTableId);
		}

		public async Task<List<CourseTime>> GetAllAsync(int courseId)
		{
			return await _context.CourseTimes
				.Include(cs => cs.Course)
				.Include(cs => cs.TimeTable)
				.Where(cs => cs.CourseId == courseId).ToListAsync();
		}

		public async Task<int> AddAsync(CourseTime model)
		{
			await _context.CourseTimes.AddAsync(model);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(int courseId, int timeTableId)
		{
			var model = await GetByIdAsync(courseId, timeTableId);
			if (model != null)
			{
				_context.CourseTimes.Remove(model);
				return await _context.SaveChangesAsync();
			}

			return 0;
		}

		// 修改
		public async Task<int> UpdateAsync(CourseTime modelParam)
		{
			var model = await _context.CourseTimes.FirstOrDefaultAsync(x => x.CourseId == modelParam.CourseId && x.TimeTableId == modelParam.TimeTableId);
			if (model == null) return 0;
			//model.StartTime = modelParam.StartTime;
			//model.EndTime = modelParam.EndTime;
			//model.Weekday = modelParam.Weekday;
			model.CourseId = modelParam.CourseId;
			model.TimeTableId = modelParam.TimeTableId;
			model.DateDay = modelParam.DateDay;
			return await _context.SaveChangesAsync();
		}
	}
}
