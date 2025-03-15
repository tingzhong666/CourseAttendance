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
		public async Task<ActionResult<List<CourseResponseDto>>> GetCourses()
		{
			var courses = await _courseRepository.GetAllAsync();
			return Ok(courses.ToList().Select(x => x.ToResponseDto()));
		}

		// 获取单个 按id
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<CourseResponseDto>> GetCourse(int id)
		{
			var course = await _courseRepository.GetByIdAsync(id);

			if (course == null)
			{
				return NotFound();
			}

			return Ok(course.ToResponseDto());
		}

		// 创建
		[HttpPost]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult> CreateCourse([FromBody] CourseRequestDto course)
		{
			var model = course.ToModel();
			if (model == null) return BadRequest("创建失败");
			var res = await _courseRepository.AddAsync(model);
			if (res == 0) return BadRequest("创建失败");

			model = await _courseRepository.GetByIdAsync(model.Id);


			return CreatedAtAction(nameof(GetCourse), new { id = model.Id }, model.ToResponseDto());
		}

		// 更新
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequestDto dto)
		{
			var model = dto.ToModel();
			model.Id = id;

			var res = await _courseRepository.UpdateAsync(model);
			if (res == 0)
				return BadRequest("更新失败");

			return NoContent();
		}

		// 删除
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			var res = await _courseRepository.DeleteAsync(id);
			if (res == 0)
				return BadRequest("删除失败");
			return NoContent();
		}
	}
}
