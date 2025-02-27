using CourseAttendance.DtoModel;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseAttendance.Controllers
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
		//[HttpPut("profile-slef")]
		//[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		//public async Task<IActionResult> UpdateProfileSelf(User user)
		//{
		//	var result = await _userManager.UpdateAsync(user);
		//	if (!result.Succeeded)
		//	{
		//		return BadRequest(result.Errors);
		//	}
		//	return NoContent();
		//}


		
		public  async Task<IdentityResult> UpdateProfileSelf(UpdateProfileReqDto user)
		{
			var result = await _userRepository.UpdateAsync(user.ToUserModel());
			return result;
		}
		#endregion

		#region 超管
		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateProfile(User user)
		{
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return NoContent();
		}
		#endregion

		#region 教务处
		#endregion

		#region 老师
		#endregion

		#region 学生
		#endregion

		// GET: api/users
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetUsers()
		{
			var users = await _userManager.Users.ToListAsync();
			return Ok(new { code = 123, users });
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<User>> GetUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}

		// POST: api/users
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

		// GET: api/users/profile
		[HttpGet("profile")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<User>> GetProfile()
		{
			var userId = User.FindFirst("id")?.Value; // 假设用户ID存储在Claims中
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}


	}
}
