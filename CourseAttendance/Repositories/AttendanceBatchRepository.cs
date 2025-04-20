using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Repositories
{
	public class AttendanceBatchRepository
	{
		private readonly AppDBContext _context;

		public AttendanceBatchRepository(AppDBContext context)
		{
			_context = context;
		}

		/// <summary>
		/// 增
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> AddAsync(AttendanceBatch model)
		{
			await _context.AttendanceBatchs.AddAsync(model);
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
			_context.AttendanceBatchs.Remove(model);
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 改
		/// </summary>
		/// <param name="modelParam"></param>
		/// <returns></returns>
		public async Task<int> UpdateAsync(AttendanceBatch modelParam)
		{
			var model = await GetByIdAsync(modelParam.Id);
			if (model == null) return 0;

			model.UpdatedAt = DateTime.Now;
			model.CheckMethod = modelParam.CheckMethod;
			model.StartTime = modelParam.StartTime;
			model.EndTime = modelParam.EndTime;
			model.PassWord = modelParam.PassWord;
			model.QRCode = modelParam.QRCode;
			model.CourseId = modelParam.CourseId;
			return await _context.SaveChangesAsync();
		}
		/// <summary>
		/// 查 一个
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public async Task<AttendanceBatch?> GetByIdAsync(int id)
		{
			var model = await _context.AttendanceBatchs
				.Include(x => x.Attendances)
				.Include(x => x.Course)
				.FirstOrDefaultAsync(x => x.Id == id);
			return model;
		}


		public async Task<(List<AttendanceBatch> queryRes, int total)> GetAllAsync(AttendanceBatchReqQueryDto query)
		{
			var queryable = _context.AttendanceBatchs.AsQueryable();
			if (query.q != null && query.q != "")
				queryable = queryable.Where(x => x.Course.Name.Contains(query.q));

			// 老师id 与 老师名 优先id
			if (query.TeacherId != null && query.TeacherId.Count != 0)
				queryable = queryable.Where(x => query.TeacherId.Contains(x.Course.Teacher.UserId));
			if (query.TeacherName != null && query.TeacherName != "" && query.TeacherId == null)
				queryable = queryable.Where(x => x.Course.Teacher.User.Name.Contains(query.TeacherName));

			// 时间范围
			if (query.StartTime != null)
				queryable = queryable.Where(x => query.StartTime < x.StartTime);
			if (query.EndTime != null)
				queryable = queryable.Where(x => x.EndTime < query.EndTime);

			// 大专业
			if (query.MajorsCategoryId != null)
			{
				queryable = queryable.Where(x => x.Course.MajorsSubcategory.MajorsCategoriesId == query.MajorsCategoryId);
			}
			// 小专业
			if (query.MajorsSubcategoriesId != null)
			{
				queryable = queryable.Where(x => x.Course.MajorsSubcategoryId == query.MajorsSubcategoriesId);
			}

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
				.Include(x => x.Attendances)
				.Include(x => x.Course)
				.ToListAsync();

			var total = queryRes.Count;
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return (queryRes, total);
		}

	}
}
