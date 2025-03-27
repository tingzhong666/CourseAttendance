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

		public CourseController(CourseRepository courseRepository)
		{
			_courseRepository = courseRepository;
		}

		// 获取所有
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<ApiResponse<List<CourseResponseDto?>>>> GetCourses()
		{
			var courses = await _courseRepository.GetAllAsync();
			return Ok(new ApiResponse<List<CourseResponseDto?>> { Code = 1, Msg = "", Data = courses.ToList().Select(x => x.ToResponseDto()).ToList() });
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
			var model = course.ToModel();
			if (model == null)
				return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "创建失败", Data = null });
			var res = await _courseRepository.AddAsync(model);
			if (res == 0) 
				return Ok(new ApiResponse<CourseResponseDto?> { Code = 2, Msg = "创建失败", Data = null });

			model = await _courseRepository.GetByIdAsync(model.Id);


				return Ok(new ApiResponse<CourseResponseDto?> { Code = 1, Msg = "创建失败", Data = model.ToResponseDto() });
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

				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		// 删除
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(int id)
		{
			var res = await _courseRepository.DeleteAsync(id);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "删除失败", Data = null });
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}
	}
}
