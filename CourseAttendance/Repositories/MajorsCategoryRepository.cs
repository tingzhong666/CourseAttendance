using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class MajorsCategoryRepository
	{
		private readonly AppDBContext _context;

		public MajorsCategoryRepository(AppDBContext context)
		{
			_context = context;
		}

		/// <summary>
		/// 增
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> AddAsync(MajorsCategory model)
		{
			await _context.MajorsCategories.AddAsync(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 删
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<int> DelAsync(int id)
		{
			var model = await GetByIdAsync(id);
			if (model == null) return 0;
			_context.MajorsCategories.Remove(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 改
		/// </summary>
		/// <param name="modelParam"></param>
		/// <returns></returns>
		public async Task<int> UpdateAsync(MajorsCategory modelParam)
		{
			var model = await GetByIdAsync(modelParam.Id);
			if (model == null) return 0;

			model.Name = modelParam.Name;
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 查 一个
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<MajorsCategory?> GetByIdAsync(int id)
		{
			var model = await _context.MajorsCategories.FirstOrDefaultAsync(x => x.Id == id);
			return model;
		}

		public async Task<(List<MajorsCategory> queryRes, int total)> GetAllAsync(ReqQueryDto query)
		{
			var queryable = _context.MajorsCategories.AsQueryable();
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x => x.Name.Contains(query.q));

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
				.Include(x => x.MajorsSubcategories)
				.ToListAsync();

			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (queryRes, total);
		}
	}
}
