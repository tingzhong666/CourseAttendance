using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Enums;
using CourseAttendance.mapper;
using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseAttendance.Controllers
{
	[Route("api/attendance")]
	[ApiController]
	public class AttendanceController : ControllerBase
	{
		private readonly AttendanceRepository _attendanceRepository;
		private readonly CourseRepository _courseRepository;

		public AttendanceController(AttendanceRepository attendanceRepository, CourseRepository courseRepository)
		{
			_attendanceRepository = attendanceRepository;
			_courseRepository = courseRepository;
		}

		/// <summary>
		/// 获取列表
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<List<AttendanceResponseDto>>>> GetAttendances([FromQuery] AttendanceFilter filter)
		{
			var models = await _attendanceRepository.GetAllAsync();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 老师与学生只返回与自身相关的
			if (User.IsInRole("Teacher"))
			{
				models = models.Where(x => x.Course.TeacherUserId == userId).ToList();
			}
			if (User.IsInRole("Student"))
			{
				models = models.Where(x => x.StudentId == userId).ToList();
			}

			// 筛选
			if (filter.CourseId != null)
			{
				models = models.Where(x => x.Course.Id == filter.CourseId).ToList();
			}
			if (filter.StudentId != null)
			{
				models = models.Where(x => x.StudentId == filter.StudentId).ToList();
			}
			if (filter.StartDate != null)
			{
				models = models.Where(x => filter.StartDate <= x.CreatedAt).ToList();
			}
			if (filter.EndDate != null)
			{
				models = models.Where(x => x.CreatedAt <= filter.StartDate).ToList();
			}


			return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 1, Msg = "", Data = models.Select(x => x.ToDto()).ToList() });
		}

		/// <summary>
		/// 获取单个详情 ID获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<AttendanceResponseDto>>> GetAttendance(int id)
		{
			var model = await _attendanceRepository.GetByIdAsync(id);
			if (model == null)
			{
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = "操作失败，未找到此考勤信息", Data = null });
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 老师与学生只返回与自身相关的
			if (User.IsInRole("Teacher") && model.Course.TeacherUserId != userId)
			{
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = "操作失败，当前用户无权限查看", Data = null });
			}
			if (User.IsInRole("Student") && model.StudentId != userId)
			{
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = "操作失败，当前用户无权限查看", Data = null });
			}
			return Ok(new ApiResponse<AttendanceResponseDto> { Code = 1, Msg = "", Data = model.ToDto() });
		}

		/// <summary>
		/// 创建考勤信息
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="checkMethod"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<List<AttendanceResponseDto>>>> CreateAttendance([FromBody] AttendanceCreateRequestDto dto)
		{
			var courseModel = await _courseRepository.GetByIdAsync(dto.CourseId);
			if (courseModel == null)
			{
				return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，未找到此课程信息", Data = null });
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 如果是老师，则验证当前课程是否有权限
			if (User.IsInRole("Teacher") && courseModel.TeacherUserId != userId)
			{
				return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，当前用户无权限发布此课程考勤", Data = null });
			}


			var models = courseModel.CourseStudents.Select(x => dto.ToModel(x.StudentId)).ToList();
			foreach (var item in models)
			{
				var res = await _attendanceRepository.AddAsync(item);
				if (res == 0)
					return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，某个考勤创建失败", Data = null });
			}

			models = await _attendanceRepository.GetAllAsync();


			return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 1, Msg = "", Data = models.Select(x => x.ToDto()).ToList() });
		}

		/// <summary>
		/// 修改考勤状态
		/// </summary>
		/// <param name="id"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpPut]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateAttendanceStatus([FromQuery] int id, [FromQuery] AttendanceStatus status)
		{
			var model = await _attendanceRepository.GetByIdAsync(id);
			if (model == null)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，未找到此考勤信息", Data = null });
			}
			// 老师身份 验证是否有权限
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 如果是老师，则验证当前课程是否有权限
			if (User.IsInRole("Teacher") && model.Course.TeacherUserId != userId)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，当前用户无权限修改此课程考勤", Data = null });
			}

			model.Status = status;
			var res = await _attendanceRepository.UpdateAsync(model);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，更新失败", Data = null });

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 学生打卡
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("sign-in")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<object>>> SignIn([FromBody] AttendanceRequestDto dto)
		{
			var model = await _attendanceRepository.GetByIdAsync(dto.Id);
			if (model == null)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，未找到此考勤信息", Data = null });
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 如果是老师，则验证当前课程是否有权限
			if (model.StudentId != userId)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，当前用户无权限执行此操作", Data = null });
			}

			//model = dto.ToModel();
			// 打卡逻辑
			var isTimeRange = model.CreatedAt <= DateTime.Now && DateTime.Now <= model.EndTime;
			// 时间在范围内为正常，否则缺席，请假只能老师手动改
			if (!isTimeRange)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，超出时间", Data = null });
			if (model.CreatedAt != model.UpdatedAt)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，已经执行过操作", Data = null });
			switch (model.CheckMethod)
			{
				case CheckMethod.Normal:
					break;
				//case CheckMethod.Location:
				//break;
				case CheckMethod.Password:
					if (dto.PassWord != model.PassWord)
						return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，密码不对", Data = null });
					break;
				default:
					return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，未知打卡方式", Data = null });
			}

			model.Status = AttendanceStatus.None;

			var res = await _attendanceRepository.UpdateAsync(model);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，打卡失败", Data = null });

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteAttendance(int id)
		{
			var attendance = await _attendanceRepository.GetByIdAsync(id);
			if (attendance == null)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，未找到此考勤信息", Data = null });
			}

			var res = await _attendanceRepository.DeleteAsync(id);
			if (res == 0)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "操作失败，删除失败", Data = null });

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		//[HttpGet("filter")]
		//[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		//public async Task<ActionResult<IEnumerable<Attendance>>> FilterAttendances([FromQuery] AttendanceFilter filter)
		//{
		//	var filteredAttendances = await _attendanceRepository.FilterAttendancesAsync(filter);
		//	return Ok(filteredAttendances);
		//}
	}
}
