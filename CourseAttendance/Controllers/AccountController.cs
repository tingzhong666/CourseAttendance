using CourseAttendance.Repositories.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseAttendance.Controllers
{
	[Route("api/account")]
	[ApiController]
	public class AccountController: ControllerBase
	{
		private readonly UserManager<UserRepository> _userManager;
		public AccountController(UserManager<UserRepository> userManager)
		{
			_userManager = userManager;
		}
	}
}
