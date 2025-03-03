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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseAttendance.Controllers.Account
{
	[Route("api/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		protected readonly UserManager<User> _userManager;
		protected readonly UserRepository _userRepository;
		protected readonly TokenService _tokenService;
		protected readonly SignInManager<User> _signInManager;

		public AccountController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, UserRepository userRepository)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
			_userRepository = userRepository;
		}

		#region 通用
		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("login")]
		public async Task<ActionResult<LoginRes>> Login(LoginModel model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName);
			if (user == null)
			{
				return Unauthorized("无效的工号或密码。");
			}
			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			//var result = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return Unauthorized("无效的工号或密码。");
			}

			// 生成 JWT 令牌
			var token = await _tokenService.CreateToken(user);
			return Ok(new LoginRes { Token = token, UserName = model.UserName });
		}

		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[NonAction]
		public async Task<IdentityResult> UpdateProfileSelf(UpdateProfileReqDto user)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return IdentityResult.Failed([new IdentityError { Description = "获取当前用户ID失败" }]);
			var model = user.ToUserModel();
			model.Id = userId;
			var result = await _userRepository.UpdateAsync(user.ToUserModel());
			return result;
		}

		/// <summary>
		/// 获取所有用户
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			var users = await _userManager.Users.ToListAsync();
			var res = users.Select(x => x.ToGetUsersResDto()).ToList();
			return Ok(res);
		}


		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public virtual async Task<ActionResult> GetUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user.ToGetUsersResDto());
		}


		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public virtual async Task<ActionResult> GetProfile()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return BadRequest("获取当前用户ID失败");
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			return Ok(user.ToGetUsersResDto());
		}


		/// <summary>
		/// 密码修改本身
		/// </summary>
		/// <param name="reqDto"></param>
		/// <returns></returns>
		[HttpPut("change-password-self")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<IActionResult> ChangePasswordSelf(ChangePasswordSelfReqDto reqDto)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return BadRequest("未携带id信息");

			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return BadRequest("未找到此用户");

			if (reqDto.NewPassword != reqDto.ConfirmPassword)
				return BadRequest("新密码和确认密码不匹配。");

			var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return NoContent();
		}
		#endregion

		#region 超管
		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[NonAction]
		public async Task<IdentityResult> UpdateProfile(UpdateProfileReqDto dto, string id)
		{
			var model = dto.ToUserModel();
			model.Id = id;
			var result = await _userRepository.UpdateAsync(dto.ToUserModel());
			return result;
		}


		/// <summary>
		/// 创建
		/// </summary>
		/// <returns></returns>
		[NonAction]
		public async Task<User> CreateUser(CreateUserReqDto dto)
		{
			var model = dto.ToModel();
			await _userRepository.AddAsync(dto.ToModel(), dto.PassWord);
			return model;
		}


		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[NonAction]
		public async Task<IdentityResult> DeleteUser(string id)
		{
			return await _userRepository.DeleteAsync(id);
		}


		/// <summary>
		/// 密码修改指定用户 超管
		/// </summary>
		/// <param name="reqDto"></param>
		/// <returns></returns>
		[HttpPut("change-password")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ChangePassword(ChangePasswordReqDto reqDto)
		{

			var user = await _userRepository.GetByIdAsync(reqDto.UserId);
			if (user == null)
				return BadRequest("未找到此用户");

			if (reqDto.NewPassword != reqDto.ConfirmPassword)
				return BadRequest("新密码和确认密码不匹配。");

			var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return NoContent();
		}
		#endregion
	}
}
