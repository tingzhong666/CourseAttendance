using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Enums;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.Model;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using CourseAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace CourseAttendance.Controllers
{
	[Route("api/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly UserRepository _userRepository;
		private readonly TokenService _tokenService;
		private readonly SignInManager<User> _signInManager;
		private readonly UserService _userService;
		private readonly AppDBContext _context;

		public AccountController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, UserRepository userRepository, UserService userService, AppDBContext context)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
			_userRepository = userRepository;
			_userService = userService;
			_context = context;
		}

		#region 通用

		// 测试
		[HttpGet("test")]
		public async Task<ActionResult<ApiResponse<object>>> Test()
		{
			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 登录状态验证
		/// </summary>
		/// <returns></returns>
		[HttpGet("check")]
		public async Task<ActionResult<ApiResponse<object>>> Check()
		{
			if (User.Identity?.IsAuthenticated ?? false)
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			else
				return Ok(new ApiResponse<object> { Code = 2, Msg = "未登录", Data = null });

		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("login")]
		public async Task<ActionResult<ApiResponse<LoginRes>>> Login(LoginModel model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName);
			if (user == null)
			{
				return Ok(new ApiResponse<object> { Code = 3, Msg = "无效的工号或密码", Data = null });
			}
			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			//var result = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 3, Msg = "无效的工号或密码", Data = null });
			}

			// 生成 JWT 令牌
			var token = await _tokenService.CreateToken(user);
			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = new LoginRes { Token = token, UserName = model.UserName } });
		}

		/// <summary>
		/// 获取所有用户
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<ApiResponse<ListDto<GetUserResDto>>>> GetUsers([FromQuery] AccountReqQueryDto query)
		{
			var users = _userManager.Users;


			// 查询 姓名/工号/学号
			if (query.q != null && query.q != "")
				users = users.Where(x => x.Name.Contains(query.q) || x.UserName.Contains(query.q));


			// 创建时间排序
			if (query.SortCreateTime != null && query.SortCreateTime == 1) // 降
			{
				users = users.OrderByDescending(x => x.CreatedAt);
			}
			if (query.SortCreateTime != null && query.SortCreateTime == 0) // 升
			{
				users = users.OrderBy(x => x.CreatedAt);
			}

			// 执行查询
			var queryRes = await users.ToListAsync();

			var queryRes2 = new List<User>();
			// 筛选身份
			if (query.Roles != null && query.Roles.Count != 0)
			{
				foreach (var item in queryRes)
				{
					var roles = await _userRepository._userManager.GetRolesAsync(item);
					if (!roles.Any(x => query.Roles.Select(v => v.ToString()).Contains(x))) continue;
					queryRes2.Add(item);
				}
			}
			else queryRes2 = queryRes;


			var total = queryRes2.Count();
			// 分页
			queryRes2 = queryRes2.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();


			var resDtos = new List<GetUserResDto>();
			foreach (var model in queryRes2)
			{
				var resDto = await _userService.ModelToDtoAsync(model);
				resDtos.Add(resDto);
			}
			return Ok(new ApiResponse<ListDto<GetUserResDto>> { Code = 1, Msg = "", Data = new ListDto<GetUserResDto> { DataList = resDtos, Total = total } });
		}


		/// <summary>
		/// 获取指定用户信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<GetUserResDto>>> GetUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(new ApiResponse<GetUserResDto> { Code = 1, Msg = "", Data = await _userService.ModelToDtoAsync(user) });
		}

		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-self")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<GetUserResDto>>> GetProfileSlef()
		{
			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return Ok(new ApiResponse<GetUserResDto> { Code = 4, Msg = "获取当前用户ID失败", Data = null });
			var user = await _userManager.FindByNameAsync(userName);
			if (user == null)
			{
				return Ok(new ApiResponse<GetUserResDto> { Code = 5, Msg = "获取当前用户信息失败", Data = null });
			}

			return Ok(new ApiResponse<GetUserResDto> { Code = 1, Msg = "", Data = await _userService.ModelToDtoAsync(user) });
		}


		/// <summary>
		/// 密码修改本身
		/// </summary>
		/// <param name="reqDto"></param>
		/// <returns></returns>
		[HttpPut("change-password-self")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<object>>> ChangePasswordSelf(ChangePasswordSelfReqDto reqDto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
				if (userName == null)
					throw new Exception("未携带用户名信息");

				var user = await _userRepository._userManager.FindByNameAsync(userName);
				if (user == null)
					throw new Exception("未找到此用户");

				if (reqDto.NewPassword != reqDto.ConfirmPassword)
					throw new Exception("新密码和确认密码不匹配");

				var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
				if (!result.Succeeded)
					throw new Exception(result.Errors.ToString());

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}


		/// <summary>
		/// 更新用户信息 自身
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPut("update-user-self")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<object>>> UpdateSelf([FromBody] UpdateProfileSelfReqDto dto)
		{
			try
			{
				var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (id == null) throw new Exception("未携带ID信息");

				var user = await _userRepository.GetByIdAsync(id);
				if (user == null)
					throw new Exception("未找到此用户");

				var roles = await _userRepository._userManager.GetRolesAsync(user);
				var res = await _userService.UpdateAsync(new UpdateProfileReqDto
				{
					UserName = dto.UserName,
					Roles = roles.Select(x => Enum.Parse<UserRole>(x)).ToList(),
					CreateAcademicExt = dto.CreateAcademicExt,
					CreateAdminExt = dto.CreateAdminExt,
					CreateStudentExt = dto.CreateStudentExt,
					CreateTeacherExt = dto.CreateTeacherExt,
					Email = dto.Email,
					Name = dto.Name,
					Phone = dto.Phone
				}, id);
				if (res != null) throw new Exception(res);

				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}
		#endregion

		#region 超管 教务处
		/// <summary>
		/// 创建
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<GetUserResDto>>> CreateUser([FromBody] CreateUserReqDto dto)
		{
			try
			{
				if (!User.IsInRole(UserRole.Admin.ToString()) && dto.Roles.Contains(UserRole.Admin))
					throw new Exception("权限不够");

				var id = await _userService.CreateUserAsync(dto);

				return Ok(new ApiResponse<GetUserResDto> { Code = 1, Msg = "", Data = await _userService.GetInfoById(id) });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<GetUserResDto> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string id)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var user = await _userRepository.GetByIdAsync(id);
				if (user == null)
					throw new Exception("未找到此用户");

				var roles = await _userRepository._userManager.GetRolesAsync(user);
				if (!User.IsInRole(UserRole.Admin.ToString()) && roles.Contains(UserRole.Admin.ToString()))
					throw new Exception("权限不够");

				var res = await _userRepository.DeleteAsync(id);
				if (!res.Succeeded)
					throw new Exception("删除失败");

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "删除成功", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}


		/// <summary>
		/// 密码修改指定用户
		/// </summary>
		/// <param name="reqDto"></param>
		/// <returns></returns>
		[HttpPut("change-password")]
		[Authorize(Roles = "Admin,Academic")]
		public async Task<ActionResult<ApiResponse<object>>> ChangePassword(ResetPasswordReqDto reqDto)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var user = await _userRepository.GetByIdAsync(reqDto.UserId);
				if (user == null)
					throw new Exception("未找到此用户");

				var roles = await _userRepository._userManager.GetRolesAsync(user);
				if (!User.IsInRole(UserRole.Admin.ToString()) && roles.Contains(UserRole.Admin.ToString()))
					throw new Exception("权限不够");

				if (reqDto.NewPassword != reqDto.ConfirmPassword)
					throw new Exception("新密码和确认密码不匹配");

				//var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
				var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
				var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, reqDto.NewPassword);
				if (!result.Succeeded)
					throw new Exception(result.Errors.ToString());

				await transaction.CommitAsync();
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}

		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPut("update-user")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] UpdateProfileReqDto dto, string id)
		{
			try
			{
				var res = await _userService.UpdateAsync(dto, id);
				if (res != null) throw new Exception(res);
				return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
			}
			catch (Exception err)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = err.Message, Data = null });
			}
		}


		#endregion
	}
}
