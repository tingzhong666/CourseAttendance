using CourseAttendance.DtoModel;
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
		private readonly UserManager<User> _userManager;
		private readonly TokenService _tokenService;
		private readonly SignInManager<User> _signInManager;

		public AccountController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
		}

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

		// PUT: api/users/profile
		[HttpPut("profile")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<IActionResult> UpdateProfile(User user)
		{
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return NoContent();
		}

		// POST: api/account/login
		[HttpPost("login")]
		public async Task<ActionResult<string>> Login(LoginModel model)
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
			return Ok(new { Token = token, model.UserName });
		}
	}
}
