﻿using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.DtoModel.ResDtos;

namespace CourseAttendance.Controllers.Account
{
	[Route("api/teacher")]
	[ApiController]
	public class TeacherController : ControllerBase
	{
		protected readonly UserRepository _userRepository;
		protected readonly TeacherRepository _teacherRepository;

		public TeacherController(UserRepository userRepository, TeacherRepository teacherRepository)
		{
			_userRepository = userRepository;
			_teacherRepository = teacherRepository;
		}



		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile-slef")]
		[Authorize(Roles = "Teacher")]
		public async Task<IActionResult> UpdateProfileSelf(UpdateProfileTeacherReqDto user)
		{
			var result = await AccountController.UpdateProfileSelf(user, this, _userRepository);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return BadRequest("获取当前用户名失败");
			var userModel = await _userRepository._userManager.FindByNameAsync(userName);
			if (userModel == null)
				return BadRequest("获取当前用户Id失败");

			var model = user.ToTeacherModel();
			model.UserId = userModel.Id;
			await _teacherRepository.UpdateAsync(model);

			return NoContent();
		}


		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileTeacherReqDto user, [FromQuery] string id)
		{
			var result = await AccountController.UpdateProfile(user, id, _userRepository);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}


			var model = user.ToTeacherModel();
			model.UserId = id;
			await _teacherRepository.UpdateAsync(model);

			return NoContent();
		}

		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<GetTeacherResDto>> GetUser(string id)
		{
			var user = await _userRepository._userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var teacher = await _teacherRepository.GetByIdAsync(id);
			if (teacher == null)
			{
				return NotFound();
			}
			return Ok(await teacher.ToGetTeacherResDto(user, _userRepository));
		}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-slef")]
		[Authorize(Roles = "Teacher")]
		public async Task<ActionResult<GetTeacherResDto>> GetProfileSlef()
		{
			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return BadRequest("获取当前用户ID失败");
			var user = await _userRepository._userManager.FindByNameAsync(userName);
			if (user == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			var teacher = await _teacherRepository.GetByIdAsync(user.Id);
			if (teacher == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			return Ok(await teacher.ToGetTeacherResDto(user, _userRepository));
		}


		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> CreateUser(CreateUserTeacherReqDto dto)
		{
			var userModel = await AccountController.CreateUser(dto, _userRepository);
			if (userModel == null) return BadRequest("创建失败");

			var resRole = await _userRepository._userManager.AddToRoleAsync(userModel, "Teacher");

			var teacherModel = dto.ToModel();
			teacherModel.UserId = userModel.Id;
			var result = await _teacherRepository.AddAsync(teacherModel);
			if (result == 0 || !resRole.Succeeded)
			{
				var res = await _userRepository.DeleteAsync(userModel.Id);
				if (!res.Succeeded) return BadRequest("创建失败");
				return BadRequest("创建失败");
			}

			teacherModel = await _teacherRepository.GetByIdAsync(userModel.Id);

			return CreatedAtAction(nameof(GetUser), new { id = userModel.Id }, await teacherModel.ToGetTeacherResDto(userModel,_userRepository));
		}


		///// <summary>
		///// 删除
		///// </summary>
		///// <param name="id"></param>
		///// <returns></returns>
		//[HttpDelete("{id}")]
		//[Authorize(Roles = "Admin")]
		//public async Task<ActionResult> DeleteUser(string id)
		//{
		//	var res = await AccountController.DeleteUser(id, _userRepository);
		//	if (!res.Succeeded) return BadRequest("删除失败");

		//	var res2 = await _teacherRepository.DeleteAsync(id);
		//	if (res2 == 0) return BadRequest("删除失败");
		//	return Ok(res);
		//}
	}
}
