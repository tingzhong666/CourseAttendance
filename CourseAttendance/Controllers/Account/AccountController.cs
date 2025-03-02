using CourseAttendance.DtoModel;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
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
		#endregion

		#region 超管
		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public async Task<IdentityResult> UpdateProfile(UpdateProfileReqDto user, string id)
		{
			var model = user.ToUserModel();
			model.Id = id;
			var result = await _userRepository.UpdateAsync(user.ToUserModel());
			return result;
		}
		#endregion


		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="user"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<User>> CreateUser(User user, string password)
		{
			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
		}

		// PUT: api/users/{id}
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateUser(string id, User user)
		{
			if (id != user.Id)
			{
				return BadRequest();
			}

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return NoContent();
		}

		// DELETE: api/users/{id}
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return NoContent();
		}

		// PUT: api/users/change-password
		[HttpPut("change-password")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			var userId = User.FindFirst("id")?.Value; // 假设用户ID存储在Claims中
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return NotFound();
			}

			if (model.NewPassword != model.ConfirmPassword)
			{
				return BadRequest("新密码和确认密码不匹配。");
			}

			var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return NoContent();
		}



	}
}
