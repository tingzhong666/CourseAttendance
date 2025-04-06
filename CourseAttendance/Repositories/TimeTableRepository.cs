using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class TimeTableRepository
	{
		private readonly AppDBContext _context;

		public TimeTableRepository(AppDBContext context)
		{
			_context = context;
		}


		/// <summary>
		/// 增
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> Add(TimeTable model)
		{
			await _context.TimeTables.AddAsync(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 删
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<int> Del(int id)
		{
			var model = await GetById(id);
			if (model == null) return 0;
			_context.TimeTables.Remove(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 改
		/// </summary>
		/// <param name="modelParam"></param>
		/// <returns></returns>
		public async Task<int> Update(TimeTable modelParam)
		{
			var model = await GetById(modelParam.Id);
			if (model == null) return 0;

			model.Name = modelParam.Name;
			model.Start = modelParam.Start;
			model.End = modelParam.End;
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 查 一个
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<TimeTable?> GetById(int id)
		{
			var model = await _context.TimeTables.FirstOrDefaultAsync(x => x.Id == id);
			return model;
		}

		/// <summary>
		/// 查所有
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<(List<TimeTable> queryRes, int total)> GetAllAsync(ReqQueryDto query)
		{
			var queryable = _context.TimeTables.AsQueryable();
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x => x.Name.Contains(query.q));

			// 执行查询
			var queryRes = await queryable
				.ToListAsync();

			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (queryRes, total);
		}

	}
}
