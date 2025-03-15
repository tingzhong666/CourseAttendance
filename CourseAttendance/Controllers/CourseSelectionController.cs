using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.Model;
using CourseAttendance.Repositories;
using CourseAttendance.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseAttendance.Controllers
{
	[Route("api/course-selection")]
	[ApiController]
	public class CourseSelectionController : ControllerBase
	{
		public readonly CourseStudentRepository _courseStudentRepository;
		public readonly StudentRepository _studentRepository;
		public readonly CourseRepository _courseRepository;
		public readonly TeacherRepository _teacherRepository;

		public CourseSelectionController(CourseStudentRepository courseStudentRepository, StudentRepository studentRepository, CourseRepository courseRepository, TeacherRepository teacherRepository)
		{
			_courseStudentRepository = courseStudentRepository;
			_studentRepository = studentRepository;
			_courseRepository = courseRepository;
			_teacherRepository = teacherRepository;
		}

		/// <summary>
		/// 当前学生添加选课
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("add-self")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult> AddSelf([FromQuery] int courseId)
		{
			// 获取当前用户Id
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (studentId == null)
				return BadRequest("操作失败，当前Token为携带ID信息");

			var model = new CourseStudent
			{
				StudentId = studentId,
				CourseId = courseId
			};



			// 先查询是否已存在
			var isExist = await _courseStudentRepository.GetByIdsAsync(model.CourseId, model.StudentId);
			if (isExist != null)
				return BadRequest("操作失败，已经选上此课，请勿重复操作");

			// 然后验证外键是否正常
			var isExistCourse = await _courseRepository.GetByIdAsync(model.CourseId);
			if (isExistCourse == null)
				return BadRequest("操作失败，此课程不存在");
			var isExistStudent = await _studentRepository.GetByIdAsync(model.StudentId);
			if (isExistStudent == null)
				return BadRequest("操作失败，此学生不存在");

			// 添加选课数据
			var res = await _courseStudentRepository.AddAsync(model);
			if (res == 0)
				return BadRequest("操作失败，未知异常");

			return Ok();
		}

		/// <summary>
		/// 当前学生退课
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("del-self")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult> DelSelf([FromQuery] int courseId)
		{
			// 获取当前用户Id
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (studentId == null)
				return BadRequest("操作失败，当前Token为携带ID信息");


			// 先查询是否已存在
			var isExist = await _courseStudentRepository.GetByIdsAsync(courseId, studentId);
			if (isExist == null)
				return BadRequest("操作失败，没有选此课");

			// 删除选课数据
			var res = await _courseStudentRepository.DeleteAsync(courseId, studentId);
			if (res == 0)
				return BadRequest("操作失败，未知异常");


			return Ok();
		}

		/// <summary>
		/// 上级权限管理指定学生 添加选课
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		[HttpGet("add")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult> Add([FromQuery] string studentId, [FromQuery] int courseId)
		{
			var model = new CourseStudent
			{
				StudentId = studentId,
				CourseId = courseId
			};

			// 先查询是否已存在
			var isExist = await _courseStudentRepository.GetByIdsAsync(model.CourseId, model.StudentId);
			if (isExist != null)
				return BadRequest("操作失败，已经选上此课，请勿重复操作");

			// 然后验证外键是否正常
			var isExistCourse = await _courseRepository.GetByIdAsync(model.CourseId);
			if (isExistCourse == null)
				return BadRequest("操作失败，此课程不存在");
			var isExistStudent = await _studentRepository.GetByIdAsync(model.StudentId);
			if (isExistStudent == null)
				return BadRequest("操作失败，此学生不存在");

			// 添加选课数据
			var res = await _courseStudentRepository.AddAsync(model);
			if (res == 0)
				return BadRequest("操作失败，未知异常");

			return Ok();
		}
		/// <summary>
		/// 上级权限管理指定学生 退课
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		[HttpGet("del")]
		[Authorize(Roles = "Admin, Academic")]
		public async Task<ActionResult> Del([FromQuery] string studentId, [FromQuery] int courseId)
		{
			// 先查询是否已存在
			var isExist = await _courseStudentRepository.GetByIdsAsync(courseId, studentId);
			if (isExist == null)
				return BadRequest("操作失败，没有选此课");

			// 删除选课数据
			var res = await _courseStudentRepository.DeleteAsync(courseId, studentId);
			if (res == 0)
				return BadRequest("操作失败，未知异常");

			return Ok();
		}


		/// <summary>
		/// 上课表现修改
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="courseId"></param>
		/// <param name="PerformanceLevel"></param>
		/// <returns></returns>
		[HttpPut("performance")]
		[Authorize(Roles = "Admin, Academic, Teacher")]
		public async Task<ActionResult> UpdatePerformance([FromQuery] string studentId, [FromQuery] int courseId, [FromQuery] int PerformanceLevel)
		{
			// 验证是否存此选课数据
			var model = await _courseStudentRepository.GetByIdsAsync(courseId, studentId);
			if (model == null)
				return BadRequest("操作失败，不存在此选课条目");

			// 请求方是老师时 验证此课程是否是当前老师授课
			if (User.IsInRole("Academic"))
			{
				var courseModel = await _courseRepository.GetByIdAsync(courseId);
				if (courseModel == null)
					return BadRequest("操作失败，课程不存在");

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					return BadRequest("操作失败，Token未携带ID信息");

				if (courseModel.TeacherUserId != userId)
					return BadRequest("操作失败，此课程不归当前老师授课");
			}


			model.Performance = (Enums.PerformanceLevel)PerformanceLevel;

			var res = await _courseStudentRepository.UpdateAsync(model);
			if (res == 0)
				return BadRequest("操作失败，未知异常");



			return Ok();
		}


		/// <summary>
		/// 查看选课信息
		/// </summary>
		/// <returns></returns>
		[HttpGet("GetAll")]
		[Authorize(Roles = "Admin, Academic, Teacher, Student")]
		public async Task<ActionResult<List<CourseSelectionResDto>>> Get([FromQuery] bool isAll)
		{
			var models = await _courseStudentRepository.GetAllAsync();
			if (isAll)
				return Ok(models.Select(x => x.ToDto()));


			// 老师和学生，返回属于自己课程相关的
			if (User.IsInRole("Teacher"))
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					return BadRequest("操作失败，Token未携带ID信息");
				var tarcherModel = await _teacherRepository.GetByIdAsync(userId);
				if (tarcherModel == null)
					return BadRequest("操作失败，当前用户不存在");
				models = models.Where(x => tarcherModel.Courses.Select(v => v.Id).Contains(x.CourseId)).ToList();

			}
			if (User.IsInRole("Student"))
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					return BadRequest("操作失败，Token未携带ID信息");
				var stndentModel = await _studentRepository.GetByIdAsync(userId);
				if (stndentModel == null)
					return BadRequest("操作失败，当前用户不存在");
				models = models.Where(x => stndentModel.CourseStudents.Select(v => v.CourseId).Contains(x.CourseId)).ToList();

			}

			return Ok(models.Select(x => x.ToDto()));
		}
	}
}
