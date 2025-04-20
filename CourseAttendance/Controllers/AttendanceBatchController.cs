using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper;
using CourseAttendance.Model;
using CourseAttendance.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseAttendance.Controllers
{
	[Route("api/attendance-batch")]
	[ApiController]
	public class AttendanceBatchController : ControllerBase
	{
		private readonly AttendanceBatchRepository _attendanceBatchRepository;
		private readonly AppDBContext _context;
		private readonly CourseRepository _courseRepository;
		private readonly AttendanceRepository _attendanceRepository;


		public AttendanceBatchController(AttendanceBatchRepository attendanceBatchRepository, AppDBContext context, CourseRepository courseRepository, AttendanceRepository attendanceRepository)
		{
			_attendanceBatchRepository = attendanceBatchRepository;
			_context = context;
			_courseRepository = courseRepository;
			_attendanceRepository = attendanceRepository;
		}

		/// <summary>
		/// 获取列表
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<ListDto<AttendanceBatchResDto>>>> GetAll([FromQuery] AttendanceBatchReqQueryDto query)
		{
			if (query.StartTime != null)
				query.StartTime = query.StartTime?.ToLocalTime();
			if (query.EndTime != null)
				query.EndTime = query.EndTime?.ToLocalTime();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Ok(new ApiResponse<ListDto<AttendanceBatchResDto>> { Code = 2, Msg = "操作失败，Token为携带ID信息", Data = null });
			// 老师只返回与自身相关的
			if (User.IsInRole("Teacher"))
				query.TeacherId = [userId];

			var (queryRes, total) = await _attendanceBatchRepository.GetAllAsync(query);

			return Ok(new ApiResponse<ListDto<AttendanceBatchResDto>>
			{
				Code = 1,
				Msg = "",
				Data = new ListDto<AttendanceBatchResDto>
				{
					DataList = queryRes.Select(x => x.ToDto()).ToList(),
					Total = total
				}
			});
		}

		// 单个获取
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<AttendanceBatchResDto>>> GetById(int id)
		{
			try
			{
				var model = await _attendanceBatchRepository.GetByIdAsync(id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤信息");

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");

				// 老师与学生只返回与自身相关的
				if (User.IsInRole("Teacher") && model.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限查看");

				return Ok(new ApiResponse<AttendanceBatchResDto> { Code = 1, Msg = "", Data = model.ToDto() });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<AttendanceBatchResDto> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		// 创建
		[HttpPost]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<AttendanceBatchResDto>>> Create([FromBody] AttendanceBatchCreateDto dto)
		{
			var transaction = _context.Database.BeginTransaction();
			try
			{
				var courseModel = await _courseRepository.GetByIdAsync(dto.CourseId);
				if (courseModel == null)
					throw new Exception("操作失败，未找到此课程信息");

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");
				// 如果是老师，则验证当前课程是否有权限
				if (User.IsInRole("Teacher") && courseModel.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限发布此课程考勤");

				var batchModel = dto.ToModel();
				// 二维码生成
				if(dto.CheckMethod == Enums.CheckMethod.TowCode)
					batchModel.QRCode = Guid.NewGuid().ToString();

				var res = await _attendanceBatchRepository.AddAsync(batchModel);
				if (res == 0)
					throw new Exception("操作失败，考勤批次添加失败");

				batchModel = await _attendanceBatchRepository.GetByIdAsync(batchModel.Id);
				if (batchModel == null)
					throw new Exception("操作失败，考勤批次添加失败");


				var now = DateTime.Now;
				var models = courseModel.CourseStudents
					.Select(x =>
					{
						return new Attendance
						{
							CreatedAt = now,
							UpdatedAt = now,

							Status = Enums.AttendanceStatus.None,
							SignInTime = null,
							Remark = "",

							AttendanceBatchId = batchModel.Id,
							StudentId = x.StudentId,
						};
					})
					.ToList();
				foreach (var item in models)
				{
					res = await _attendanceRepository.AddAsync(item);
					if (res == 0)
						throw new Exception("操作失败，某个考勤创建失败");
				}

				await transaction.CommitAsync();
				return Ok(new ApiResponse<List<AttendanceResponseDto>> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<AttendanceResponseDto> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		// 修改
		[HttpPut]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] AttendanceBatchUpdateDto dto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var model = await _attendanceBatchRepository.GetByIdAsync(dto.Id);
				if (model == null)
					throw new Exception("操作失败，未找到此考勤批次信息");

				// 老师身份 验证是否有权限
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token未携带ID信息");

				// 如果是老师，则验证当前课程是否有权限
				if (User.IsInRole("Teacher") && model.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限修改此课程考勤");

				if (dto.CheckMethod != null)
					model.CheckMethod = dto.CheckMethod.Value;
				if (dto.StartTime != null)
					model.StartTime = dto.StartTime.Value;
				if (dto.EndTime != null)
					model.EndTime = dto.EndTime.Value;
				// 改变类型，且新类型是二维码，老类型不是二维码   生成二维码的随机字符串
				if(dto.CheckMethod != null && model.CheckMethod != Enums.CheckMethod.TowCode && dto.CheckMethod == Enums.CheckMethod.TowCode)
					model.QRCode = Guid.NewGuid().ToString();

				var res = await _attendanceBatchRepository.UpdateAsync(model);
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

		// 删除
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Academic,Teacher")]
		public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var attendance = await _attendanceBatchRepository.GetByIdAsync(id);
				if (attendance == null)
					throw new Exception("操作失败，未找到此考勤信息");

				// 老师身份 验证是否有权限
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
					throw new Exception("操作失败，Token为携带ID信息");

				// 如果是老师，则验证当前课程是否有权限
				if (User.IsInRole("Teacher") && attendance.Course.TeacherUserId != userId)
					throw new Exception("操作失败，当前用户无权限修改此课程考勤");


				var res = await _attendanceBatchRepository.DelAsync(id);
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
	}
}
