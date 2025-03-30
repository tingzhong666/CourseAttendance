using CourseAttendance.AppDataContext;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class WebSystemConfigRepository
	{
		private readonly AppDBContext _context;

		public WebSystemConfigRepository(AppDBContext context)
		{
			_context = context;
		}

		/// <summary>
		/// 增
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> Add(WebSystemConfig model)
		{
			await _context.WebSystemConfigs.AddAsync(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 删
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<int> Del(string k)
		{
			var model = await GetByKey(k);
			if (model == null) return 0;
			_context.WebSystemConfigs.Remove(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 改
		/// </summary>
		/// <param name="modelParam"></param>
		/// <returns></returns>
		public async Task<int> Update(WebSystemConfig modelParam)
		{
			var model = await GetByKey(modelParam.Key);
			if (model == null) return 0;

			model.Value = modelParam.Value;
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 查 一个
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<WebSystemConfig?> GetByKey(string k)
		{
			var model = await _context.WebSystemConfigs.FirstOrDefaultAsync(x => x.Key == k);
			return model;
		}
		/// <summary>
		/// 查 模糊匹配
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<List<WebSystemConfig>> GetAllByKey(string k)
		{
			var models = await _context.WebSystemConfigs.Where(x => x.Key.Contains(k)).ToListAsync();
			return models;
		}
	}
}
