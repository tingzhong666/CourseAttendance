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
	public class AcademicController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, AcademicRepository academicRepository, UserRepository userRepository) : AccountController(userManager, tokenService, signInManager, userRepository)
	{
		protected readonly AcademicRepository _academicRepository = academicRepository;


		/// <summary>
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPut("profile-slef")]
		[Authorize(Roles = "Academic")]
		public async Task<IActionResult> UpdateProfileSelf(UpdateProfileAcademicReqDto user)
		{
			var result = await base.UpdateProfileSelf(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}


			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return BadRequest("获取当前用户ID失败");
			var model = user.ToAcademicModel();
			model.UserId = userId;
			await _academicRepository.UpdateAsync(model);

			return NoContent();
		}


		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <returns></returns>
		[HttpPut("profile")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileAcademicReqDto user, [FromQuery] string id)
		{
			var result = await base.UpdateProfile(user, id);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}


			var model = user.ToAcademicModel();
			model.UserId = id;
			await _academicRepository.UpdateAsync(model);

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
			var academic = await _academicRepository.GetByIdAsync(id);
			if (academic == null)
			{
				return NotFound();
			}
			return Ok(academic.ToGetAcademicResDto(user));
		}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile")]
		[Authorize(Roles = "Academic")]
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
			var academic = await _academicRepository.GetByIdAsync(userId);
			if (academic == null)
			{
				return BadRequest("获取当前用户信息失败");
			}
			return Ok(academic.ToGetAcademicResDto(user));
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> CreateUser(CreateUserAcademicReqDto dto)
		{
			var userModel = await base.CreateUser(dto);
			if (userModel == null) return BadRequest("创建失败");

			var academicModel = dto.ToModel();
			academicModel.UserId = userModel.Id;
			var result = await _academicRepository.AddAsync(academicModel);
			if (result == 0)
			{
				await DeleteUser(userModel.Id);
				return BadRequest("创建失败");
			}
			return CreatedAtAction(nameof(GetUser), new { id = userModel.Id });
		}


		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public new async Task<ActionResult> DeleteUser(string id)
		{
			var res = await base.DeleteUser(id);
			if (!res.Succeeded) return BadRequest("删除失败");

			var res2 = await _academicRepository.DeleteAsync(id);
			if (res2 == 0) return BadRequest("删除失败");
			return Ok(res);
		}
	}
}
