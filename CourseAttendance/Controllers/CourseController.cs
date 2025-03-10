using CourseAttendance.DtoModel.ReqDtos;
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
		public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
		{
			var courses = await _courseRepository.GetAllAsync();
			return Ok(courses);
		}

		// 获取单个 按id
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<Course>> GetCourse(int id)
		{
			var course = await _courseRepository.GetByIdAsync(id);

			if (course == null)
			{
				return NotFound();
			}

			return Ok(course);
		}

		// 创建
		[HttpPost]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult<Course>> CreateCourse([FromBody] CourseRequestDto course)
		{
			var model = course.ToModel();
			await _courseRepository.AddAsync(model);
			return CreatedAtAction(nameof(GetCourse), new { id = model.Id }, course);
		}

		// 更新
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course course)
		{
			if (id != course.Id)
			{
				return BadRequest();
			}

			await _courseRepository.UpdateAsync(course);
			return NoContent();
		}

		// 删除
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			await _courseRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
