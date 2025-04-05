using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class MajorsSubcategoryRepository
	{
		private readonly AppDBContext _context;

		public MajorsSubcategoryRepository(AppDBContext context)
		{
			_context = context;
		}


		/// <summary>
		/// 增
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> AddAsync(MajorsSubcategory model)
		{
			await _context.MajorsSubcategories.AddAsync(model);
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
			_context.MajorsSubcategories.Remove(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 改
		/// </summary>
		/// <param name="modelParam"></param>
		/// <returns></returns>
		public async Task<int> UpdateAsync(MajorsSubcategory modelParam)
		{
			var model = await GetByIdAsync(modelParam.Id);
			if (model == null) return 0;

			model.Name = modelParam.Name;
			model.MajorsCategoriesId = modelParam.MajorsCategoriesId;
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 查 一个
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<MajorsSubcategory?> GetByIdAsync(int id)
		{
			var model = await _context.MajorsSubcategories.FirstOrDefaultAsync(x => x.Id == id);
			return model;
		}


		public async Task<(List<MajorsSubcategory> queryRes, int total)> GetAllAsync(MajorsSubReqQueryDto query)
		{
			var queryable = _context.MajorsSubcategories.AsQueryable();
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x => x.Name.Contains(query.q));
			if (query.MajorId != null)
				queryable = queryable.Where(x => x.MajorsCategoriesId == query.MajorId);

			// 执行查询
			var queryRes = await queryable
				.Include(x => x.MajorsCategory)
				.Include(x => x.Grades)
				.ToListAsync();

			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (queryRes, total);
		}
	}
}
