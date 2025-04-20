using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
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

		public async Task<(List<Grade> queryRes, int total)> GetAllAsync(ReqQueryDto query)
		{
			var queryable = _context.Grades.AsQueryable();
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x =>
					x.Name.Contains(query.q)  ||
					x.MajorsSubcategory.Name.Contains(query.q) ||
					x.MajorsSubcategory.MajorsCategory.Name.Contains(query.q)
				);

			// 创建时间排序
			if (query.SortCreateTime != null && query.SortCreateTime == 1) // 降
			{
				queryable = queryable.OrderByDescending(x => x.CreatedAt);
			}
			if (query.SortCreateTime != null && query.SortCreateTime == 0) // 升
			{
				queryable = queryable.OrderBy(x => x.CreatedAt);
			}

			// 执行查询
			var queryRes = await queryable
				.Include(g => g.Students)
				.ToListAsync();

			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (queryRes, total);
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
			model.Year = grade.Year;
			model.MajorsSubcategoriesId = grade.MajorsSubcategoriesId;
			model.Num = grade.Num;

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
