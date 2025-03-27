﻿using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using Microsoft.EntityFrameworkCore;
using CourseAttendance.DtoModel.ResDtos;

namespace CourseAttendance.Controllers.Account
{
	[Route("api/student")]
	[ApiController]
	public class StudentController : ControllerBase
	{
		protected readonly UserRepository _userRepository;
		protected readonly StudentRepository _studentRepository;

		public StudentController(UserRepository userRepository, StudentRepository studentRepository)
		{
			_userRepository = userRepository;
			_studentRepository = studentRepository;
		}





		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile-slef")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateProfileSelf(UpdateProfileStudentReqDto user)
		{
			var result = await AccountController.UpdateProfileSelf(user, this, _userRepository);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = result.Errors.ToString(), Data = null });
			}


			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "获取当前用户名失败", Data = null });
			var userModel = await _userRepository._userManager.FindByNameAsync(userName);
			if (userModel == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "获取当前用户Id失败", Data = null });

			var model = user.ToStudentModel();
			model.UserId = userModel.Id;
			await _studentRepository.UpdateAsync(model);

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateProfileStudentReqDto user, [FromQuery] string id)
		{
			var result = await AccountController.UpdateProfile(user, id, _userRepository);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = result.Errors.ToString(), Data = null });
			}


			var model = user.ToStudentModel();
			model.UserId = id;
			await _studentRepository.UpdateAsync(model);

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<GetStudentResDto>>> GetUser(string id)
		{
			var user = await _userRepository._userManager.FindByIdAsync(id);
			if (user == null)
			{
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "获取用户失败", Data = null });
			}
			var student = await _studentRepository.GetByIdAsync(id);
			if (student == null)
			{
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "获取用户失败", Data = null });
			}
			return Ok(new ApiResponse<GetStudentResDto> { Code = 1, Msg = "获取用户失败", Data = await student.ToGetStudentResDto(user, _userRepository) });
		}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-slef")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ApiResponse<GetStudentResDto>>> GetProfileSlef()
		{
			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "获取当前用户ID失败", Data = null });
			var user = await _userRepository._userManager.FindByNameAsync(userName);
			if (user == null)
			{
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "获取当前用户信息失败", Data = null });
			}
			var student = await _studentRepository.GetByIdAsync(user.Id);
			if (student == null)
			{
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "获取当前用户信息失败", Data = null });
			}
			return Ok(new ApiResponse<GetStudentResDto> { Code = 1, Msg = "获取用户失败", Data = await student.ToGetStudentResDto(user, _userRepository) });
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<GetStudentResDto>>> CreateUser(CreateUserStudentReqDto dto)
		{
			try
			{
				var userModel = await AccountController.CreateUser(dto, _userRepository);
				if (userModel == null)
					return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "创建失败", Data = null });

				var resRole = await _userRepository._userManager.AddToRoleAsync(userModel, "Student");

				var studentModel = dto.ToModel();
				studentModel.UserId = userModel.Id;
				var result = await _studentRepository.AddAsync(studentModel);
				if (result == 0 || !resRole.Succeeded)
				{
					var res = await _userRepository.DeleteAsync(userModel.Id);
					return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "创建失败", Data = null });
				}

				studentModel = await _studentRepository.GetByIdAsync(userModel.Id);

				return Ok(new ApiResponse<GetStudentResDto> { Code = 1, Msg = "", Data = await studentModel.ToGetStudentResDto(userModel, _userRepository) });
			}
			catch (DbUpdateException ex)
			{
				// 检查异常信息是否包含特定外键约束的名称
				if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_Students_Grades_GradeId"))
				{
					return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "创建失败，班级不存在", Data = null });
				}

				// 处理其他数据库错误
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = $"数据库错误: {ex.InnerException?.Message}", Data = null });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<GetStudentResDto> { Code = 2, Msg = "创建失败", Data = null });
			}
		}


		///// <summary>
		///// 删除
		///// </summary>
		///// <param name="id"></param>
		///// <returns></returns>
		//[HttpDelete("{id}")]
		//[Authorize(Roles = "Admin")]
		//public  async Task<ActionResult> DeleteUser(string id)
		//{
		//	var res = await AccountController.DeleteUser(id, _userRepository);
		//	if (!res.Succeeded) return BadRequest("删除失败");

		//	var res2 = await _studentRepository.DeleteAsync(id);
		//	if (res2 == 0) return BadRequest("删除失败");
		//	return Ok(res);
		//}
	}
}
