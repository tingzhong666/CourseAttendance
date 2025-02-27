using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.mapper;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
		public new async Task<IActionResult> UpdateProfileSelf(UpdateProfileReqDto user)
		{
			var result = await base.UpdateProfileSelf(user);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			await _academicRepository.UpdateAsync(user.ToAcademicModel());

			return NoContent();
		}
	}
}
