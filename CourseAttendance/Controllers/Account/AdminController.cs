using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.mapper.UserExts;

namespace CourseAttendance.Controllers.Account
{
	[Route("api/admin")]
	[ApiController]
	public class AdminController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, AdminRepository adminRepository, UserRepository userRepository) : AccountController(userManager, tokenService, signInManager, userRepository)
	{
		protected readonly AdminRepository _adminRepository = adminRepository;
		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile-slef")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> UpdateProfileSelf(UpdateProfileAdminReqDto user)
		{
			var result = await base.UpdateProfileSelf(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}


			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return BadRequest("获取当前用户ID失败");
			var model = user.ToAdminModel();
			model.UserId = userId;
			await _adminRepository.UpdateAdminAsync(model);

			return NoContent();
		}


		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileAdminReqDto user, [FromQuery] string id)
		{
			var result = await base.UpdateProfile(user, id);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}


			var model = user.ToAdminModel();
			model.UserId = id;
			await _adminRepository.UpdateAdminAsync(model);

			return NoContent();
		}



		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public override async Task<ActionResult> GetUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var admin = await _adminRepository.GetAdminByIdAsync(id);
			if (admin == null)
			{
				return NotFound();
			}
			return Ok(admin.ToGetAdminResDto(user));
		}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile")]
		[Authorize(Roles = "Admin")]
		public override async Task<ActionResult> GetProfile()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return BadRequest("获取当前用户ID失败");
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			var admin = await _adminRepository.GetAdminByIdAsync(userId);
			if (admin == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			return Ok(admin.ToGetAdminResDto(user));
		}
	}
}
