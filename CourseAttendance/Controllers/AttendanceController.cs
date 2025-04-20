using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel;
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
		private readonly AppDBContext _context;
		private readonly AttendanceRepository _attendanceRepository;
		private readonly CourseRepository _courseRepository;

		public AttendanceController(AttendanceRepository attendanceRepository, CourseRepository courseRepository, AppDBContext context)
		{
			_attendanceRepository = attendanceRepository;
			_courseRepository = courseRepository;
			_context = context;
		}

		/// <summary>
		/// 获取列表
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<ListDto<AttendanceResponseDto>>>> GetAttendances([FromQuery] AttendanceReqQueryDto query)
		{
			if (query.StartTime != null)
				query.StartTime = query.StartTime?.ToLocalTime();
			if (query.EndTime != null)
				query.EndTime = query.EndTime?.ToLocalTime();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<ListDto<AttendanceResponseDto>> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 老师与学生只返回与自身相关的
			if (User.IsInRole("Teacher"))
				query.TeacherId = [userId];
			if (User.IsInRole("Student"))
				query.StudentId = [userId];

			var (queryRes, total) = await _attendanceRepository.GetAllAsync(query);

			return Ok(new ApiResponse<ListDto<AttendanceResponseDto>>
			{
				Code = 1,
				Msg = "",
				Data = new ListDto<AttendanceResponseDto>
				{
					DataList = queryRes.Select(x => x.ToDto()).ToList(),
					Total = total
				}
			});
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
			try
			{
				var model = await _attendanceRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");

				// 老师与学生只返回与自身相关的
				if (User.IsInRole("Teacher") && model.AttendanceBatch.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限查看");
				if (User.IsInRole("Student") && model.StudentId != userId)
					throw new Exception("操作失败，当前用户无权限查看");

				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 1, Msg = "", Data = model.ToDto() });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		///// <summary>
		///// 创建考勤信息
		///// </summary>
		///// <param name="courseId"></param>
		///// <param name="checkMethod"></param>
		///// <returns></returns>
		//[HttpPost]
		//[Authorize(Roles = "Admin,Academic,Teacher")]
		//public async Task<ActionResult<ApiResponse<AttendanceResponseDto>>> CreateAttendance([FromBody] AttendanceCreateRequestDto dto)
		//{
		//	var transaction =  _context.Database.BeginTransaction();
		//	try
		//	{
		//		var courseModel = await _courseRepository.GetByIdAsync(dto.CourseId);
		//		if (courseModel == null)
		//			throw new Exception("操作失败，未找到此课程信息");

		//		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		//		if (userId == null)
		//			throw new Exception("操作失败，Token为携带ID信息");
		//		// 如果是老师，则验证当前课程是否有权限
		//		if (User.IsInRole("Teacher") && courseModel.TeacherUserId != userId)
		//			throw new Exception("操作失败，当前用户无权限发布此课程考勤");


		//		var models = courseModel.CourseStudents.Select(x => dto.ToModel(x.StudentId)).ToList();
		//		foreach (var item in models)
		//		{
		//			var res = await _attendanceRepository.AddAsync(item);
		//			if (res == 0)
		//				throw new Exception("操作失败，某个考勤创建失败");
		//		}

		//		await transaction.CommitAsync();
		//		return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 1, Msg = "", Data = null });
		//	}
		//	catch (Exception err)
		//	{
		//		await transaction.RollbackAsync();
		//		return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = err.Message, Data = null });
		//	}
		//}

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="id"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpPut]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<object>>> Update([FromQuery] int id, [FromBody] AttendanceUpdateRequestDto dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = await _attendanceRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				// 老师身份 验证是否有权限
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");

				// 如果是老师，则验证当前课程是否有权限
				if (User.IsInRole("Teacher") && model.AttendanceBatch.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限修改此课程考勤");

				model.Status = dto.Status;
				model.Remark = dto.Remark;
				var res = await _attendanceRepository.UpdateAsync(model);
				if (res == 0)
					throw new Exception("操作失败，更新失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteAttendance(int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var attendance = await _attendanceRepository.GetByIdAsync(id);
				if (attendance == null)
					throw new Exception("操作失败，未找到此考勤信息");

				// 老师身份 验证是否有权限
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");

				// 如果是老师，则验证当前课程是否有权限
				if (User.IsInRole("Teacher") && attendance.AttendanceBatch.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限修改此课程考勤");


				var res = await _attendanceRepository.DeleteAsync(id);
				if (res == 0)
					throw new Exception("操作失败，删除失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}



		// 普通考勤
		[HttpPost("/checking-in")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<object>>> Attendance([FromQuery] int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{

				var model = await _attendanceRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				if(model.StudentId != User.FindFirstValue(ClaimTypes.NameIdentifier))
					throw new Exception("操作失败，当前考勤与当前用户无关联");

				model.Status = AttendanceStatus.Ok;
				var res = await _attendanceRepository.UpdateAsync(model);
				if (res == 0)
					throw new Exception("操作失败，更新失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}
		// 密码考勤
		[HttpPost("/checking-in-pw")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<object>>> AttendancePW([FromQuery] int id, [FromBody] PasswordAttendanceRequest dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = await _attendanceRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				if (model.StudentId != User.FindFirstValue(ClaimTypes.NameIdentifier))
					throw new Exception("操作失败，当前考勤与当前用户无关联");

				if(dto.Password != model.AttendanceBatch.PassWord)
					throw new Exception("操作失败，密码错误");

				model.Status = AttendanceStatus.Ok;
				var res = await _attendanceRepository.UpdateAsync(model);
				if (res == 0)
					throw new Exception("操作失败，更新失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}
		// 二维码考勤
		[HttpPost("/checking-in-qr")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<object>>> AttendanceQRCode([FromQuery] int id, [FromBody] AttendanceQRCodeReqDto dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = await _attendanceRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				if (model.StudentId != User.FindFirstValue(ClaimTypes.NameIdentifier))
					throw new Exception("操作失败，当前考勤与当前用户无关联");

				if (dto.Code != model.AttendanceBatch.QRCode)
					throw new Exception("操作失败，二维码无效");

				model.Status = AttendanceStatus.Ok;
				var res = await _attendanceRepository.UpdateAsync(model);
				if (res == 0)
					throw new Exception("操作失败，更新失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}
	}
}
