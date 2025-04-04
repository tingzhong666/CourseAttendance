using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseAttendance.Controllers.Account
{
	[Route("api/academic")]
	[ApiController]
	public class AcademicController : ControllerBase
	{
		protected readonly UserRepository _userRepository;
		protected readonly AcademicRepository _academicRepository;

		public AcademicController(UserRepository userRepository, AcademicRepository academicRepository)
		{
			_userRepository = userRepository;
			_academicRepository = academicRepository;
		}




		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile-slef")]
		[Authorize(Roles = "Academic")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateProfileSelf(UpdateProfileAcademicReqDto user)
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

			var model = user.ToAcademicModel();
			model.UserId = userModel.Id;
			await _academicRepository.UpdateAsync(model);

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}


		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateProfileAcademicReqDto user, [FromQuery] string id)
		{
			var result = await AccountController.UpdateProfile(user, id, _userRepository);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = result.Errors.ToString(), Data = null });
			}


			var model = user.ToAcademicModel();
			model.UserId = id;
			await _academicRepository.UpdateAsync(model);

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}


		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		//[HttpGet("{id}")]
		//public async Task<ActionResult<ApiResponse<GetAcademicResDto>>> GetUser(string id)
		//{
		//	var user = await _userRepository._userManager.FindByIdAsync(id);
		//	if (user == null)
		//	{
		//		return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "没有此用户", Data = null });
		//	}
		//	var academic = await _academicRepository.GetByIdAsync(id);
		//	if (academic == null)
		//	{
		//		return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "没有此用户", Data = null });
		//	}
		//	return Ok(new ApiResponse<GetAcademicResDto> { Code = 1, Msg = "", Data = await academic.ToGetAcademicResDto(user, _userRepository) });
		//}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-slef")]
		[Authorize(Roles = "Academic")]
		public async Task<ActionResult<ApiResponse<GetAcademicResDto>>> GetProfileSlef()
		{
			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "获取当前用户ID失败", Data = null });
			var user = await _userRepository._userManager.FindByNameAsync(userName);
			if (user == null)
			{
				return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "获取当前用户信息失败", Data = null });
			}
			var academic = await _academicRepository.GetByIdAsync(user.Id);
			if (academic == null)
			{
				return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "获取当前用户信息失败", Data = null });
			}
			return Ok(new ApiResponse<GetAcademicResDto> { Code = 1, Msg = "获取当前用户信息失败", Data = await academic.ToGetAcademicResDto(user, _userRepository) });
		}

		///// <summary>
		///// 创建
		///// </summary>
		///// <param name="dto"></param>
		///// <returns></returns>
		//[HttpPost]
		//[Authorize(Roles = "Admin")]
		//public async Task<ActionResult<ApiResponse<GetAcademicResDto>>> CreateUser([FromBody] CreateUserAcademicReqDto dto)
		//{
		//	var userModel = await AccountController.CreateUser(dto, _userRepository);
		//	if (userModel == null)
		//		return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "创建失败", Data = null });

		//	var resRole = await _userRepository._userManager.AddToRoleAsync(userModel, "Academic");

		//	var academicModel = dto.ToModel();
		//	academicModel.UserId = userModel.Id;
		//	var result = await _academicRepository.AddAsync(academicModel);

		//	if (result == 0 || !resRole.Succeeded)
		//	{
		//		var res = await _userRepository.DeleteAsync(userModel.Id);
		//		if (!res.Succeeded)
		//			return Ok(new ApiResponse<GetAcademicResDto> { Code = 2, Msg = "创建失败", Data = null });
		//	}

		//	academicModel = await _academicRepository.GetByIdAsync(userModel.Id);

		//	return Ok(new ApiResponse<GetAcademicResDto> { Code = 1, Msg = "创建失败", Data = await academicModel.ToGetAcademicResDto(userModel, _userRepository) });
		//}


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

		//	var res2 = await _academicRepository.DeleteAsync(id);
		//	if (res2 == 0) return BadRequest("删除失败");
		//	return Ok(res);
		//}
	}
}
