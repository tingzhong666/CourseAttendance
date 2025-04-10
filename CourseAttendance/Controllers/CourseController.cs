using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.Model;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseAttendance.Controllers
{
	[Route("api/course")]
	[ApiController]
	public class CourseController : Controller
	{

		private readonly CourseRepository _courseRepository;
		private readonly CourseTimeRepository _courseTimeRepository;
		private readonly AppDBContext _context;

		public CourseController(CourseRepository courseRepository, CourseTimeRepository courseTimeRepository, AppDBContext context)
		{
			_courseRepository = courseRepository;
			_courseTimeRepository = courseTimeRepository;
			_context = context;
		}

		// 获取所有
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<ApiResponse<CourseResponseListDto>>> GetCourses([FromQuery] CourseReqQueryDto query)
		{
			var courses = await _courseRepository.GetAllAsync();
			if (query.q != null && query.q != "")
			{
				courses = courses.Where(x => x.Name.Contains(query.q)).ToList();
			}

			var total = courses.Count();
			// 分页
			courses = courses.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();

			return Ok(new ApiResponse<CourseResponseListDto>
			{
				Code = 1,
				Msg = "",
				Data = new CourseResponseListDto
				{
					DataList = courses.ToList().Select(x => x.ToResponseDto()).Where(x => x != null).Cast<CourseResponseDto>().ToList(),
					Total = total
				}
			});
		}

		// 获取单个 按id
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<CourseResponseDto?>>> GetCourse(int id)
		{
			var course = await _courseRepository.GetByIdAsync(id);

			if (course == null)
			{
				return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "", Data = null });
			}

			return Ok(new ApiResponse<CourseResponseDto?> { Code = 1, Msg = "", Data = course.ToResponseDto() });
		}

		// 创建
		[HttpPost]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<CourseResponseDto?>>> CreateCourse([FromBody] CourseRequestDto course)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// 课程创建
				var model = course.ToModel();
				if (model == null)
					throw new Exception("创建失败");
				var res = await _courseRepository.AddAsync(model);
				if (res == 0)
					throw new Exception("创建失败");

				// 上课时间创建
				var courseTimeModels = course.CourseTimes.Select(x =>
				{
					var modelCT = x.ToModel();
					modelCT.CourseId = model.Id;
					return modelCT;
				}).ToList();
				foreach (var courseTimeModel in courseTimeModels)
				{
					res = await _courseTimeRepository.AddAsync(courseTimeModel);
					if (res == 0)
						throw new Exception("上课时间创建失败");
				}

				model = await _courseRepository.GetByIdAsync(model.Id);

				await transaction.CommitAsync();
				return Ok(new ApiResponse<CourseResponseDto?> { Code = 1, Msg = "", Data = model.ToResponseDto() });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		// 更新
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateCourse(int id, [FromBody] CourseRequestDto dto)
		{
			var model = dto.ToModel();
			model.Id = id;

			var res = await _courseRepository.UpdateAsync(model);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "更新失败", Data = null });


			// 上课时间更新
			var courseTimeModels = model.CourseTimes;
			// 缺少的删除
			model = await _courseRepository.GetByIdAsync(model.Id);
			var delCourseTimeModels = model.CourseTimes
											.Where(x =>
												courseTimeModels.FirstOrDefault(v =>
													v.Id != x.Id && v.CourseId == x.CourseId) == null
											).ToList();
			foreach (var courseTimeModel in delCourseTimeModels)
			{
				res = await _courseTimeRepository.DeleteAsync(courseTimeModel.CourseId, courseTimeModel.TimeTableId);
				if (res == 0)
					return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "上课时间删除失败", Data = null });
			}

			// 更新与创建
			foreach (var courseTimeModel in courseTimeModels)
			{
				var courseTimeModel2 = await _courseTimeRepository.GetByIdAsync(courseTimeModel.CourseId, courseTimeModel.TimeTableId);
				// 没有就创建
				if (courseTimeModel2 == null)
				{
					res = await _courseTimeRepository.AddAsync(courseTimeModel);
					if (res == 0)
						return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "上课时间创建失败", Data = null });
				}
				// 有就更新
				else
				{
					res = await _courseTimeRepository.UpdateAsync(courseTimeModel);
					if (res == 0)
						return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "上课时间更新失败", Data = null });
				}
			}

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		// 删除
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(int id)
		{
			var model = await _courseRepository.GetByIdAsync(id);
			if (model == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "删除失败，不存在", Data = null });

			// 上课时间删除
			var courseTimeModels = model.CourseTimes;
			foreach (var courseTimeModel in courseTimeModels)
			{
				var ress = await _courseTimeRepository.DeleteAsync(courseTimeModel.CourseId, courseTimeModel.TimeTableId);
				if (ress == 0)
					return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "上课时间删除失败", Data = null });
			}

			// 课程删除
			var res = await _courseRepository.DeleteAsync(id);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "删除失败", Data = null });


			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}
	}
}
