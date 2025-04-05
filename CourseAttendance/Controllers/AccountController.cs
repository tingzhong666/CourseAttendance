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

		public AccountController(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, UserRepository userRepository, UserService userService)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_signInManager = signInManager;
			_userRepository = userRepository;
			_userService = userService;
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
		/// 更新用户信息 本身
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		//[NonAction]
		//public static async Task<IdentityResult> UpdateProfileSelf(UpdateProfileReqDto user, ControllerBase controller, UserRepository _userRepository)
		//{
		//	var userName = controller.User.FindFirst(ClaimTypes.GivenName)?.Value;
		//	if (userName == null)
		//		return IdentityResult.Failed([new IdentityError { Description = "获取当前用户名失败" }]);
		//	var userModel = await _userRepository._userManager.FindByNameAsync(userName);
		//	if (userModel == null)
		//		return IdentityResult.Failed([new IdentityError { Description = "获取当前用户Id失败" }]);

		//	var model = user.ToUserModel();
		//	model.Id = userModel.Id;
		//	var result = await _userRepository.UpdateAsync(model);
		//	return result;
		//}

		/// <summary>
		/// 获取所有用户
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<ApiResponse<ListDto<GetUserResDto>>>> GetUsers([FromQuery] ReqQueryDto query)
		{
			var users = _userManager.Users;


			// 查询 姓名/工号/学号
			if (query.q != null && query.q != "")
			{
				users = users.Where(x => x.Name.Contains(query.q));
				users = users.Where(x => x.UserName.Contains(query.q));
			}


			// 执行查询
			var queryRes = await users.ToListAsync();

			var total = queryRes.Count();
			// 分页
			queryRes = queryRes.Skip(query.Limit * (query.Page - 1)).Take(query.Limit).ToList();


			var resDtos = new List<GetUserResDto>();
			foreach (var model in queryRes)
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

		//[NonAction]
		//public async Task<GetUserResDto> ModelToDto(User model)
		//{
		//	var dto = await model.ToGetUsersResDto(_userRepository);
		//	foreach (var x in dto.Roles)
		//	{
		//		switch (x)
		//		{
		//			case UserRole.Admin:
		//				{
		//					var extModel = await _adminRepository.GetAdminByIdAsync(dto.Id);
		//					dto.GetAdminExt = extModel.ToGetAdminResDto();
		//				}
		//				break;
		//			case UserRole.Academic:
		//				{
		//					var extModel = await _academicRepository.GetByIdAsync(dto.Id);
		//					dto.GetAcademicExt = extModel.ToGetAcademicResDto();
		//				}
		//				break;
		//			case UserRole.Teacher:
		//				{
		//					var extModel = await _teacherRepository.GetByIdAsync(dto.Id);
		//					dto.GetTeacherExt = extModel.ToGetTeacherResDto();
		//				}
		//				break;
		//			case UserRole.Student:
		//				{
		//					var extModel = await _studentRepository.GetByIdAsync(dto.Id);
		//					dto.GetStudentExt = extModel.ToGetStudentResDto();
		//				}
		//				break;
		//		}
		//	}

		//	return dto;
		//}


		/// <summary>
		/// 获取用户信息 本身
		/// </summary>
		/// <returns></returns>
		[HttpGet("profile-self")]
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
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
		[Authorize(Roles = "Admin,Academic,Teacher,Student")]
		public async Task<ActionResult<ApiResponse<object>>> ChangePasswordSelf(ChangePasswordSelfReqDto reqDto)
		{
			var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;
			if (userName == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "未携带用户名信息", Data = null });

			var user = await _userRepository._userManager.FindByNameAsync(userName);
			if (user == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "未找到此用户", Data = null });

			if (reqDto.NewPassword != reqDto.ConfirmPassword)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "新密码和确认密码不匹配。", Data = null });

			var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = result.Errors.ToString(), Data = null });
			}

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}
		#endregion

		#region 超管
		/// <summary>
		/// 更新用户信息 任意 管理员
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		//[NonAction]
		//public static async Task<IdentityResult> UpdateProfile(UpdateProfileReqDto dto, string id, UserRepository _userRepository)
		//{
		//	var model = dto.ToUserModel();
		//	model.Id = id;
		//	var result = await _userRepository.UpdateAsync(model);
		//	return result;
		//}


		/// <summary>
		/// 创建
		/// </summary>
		/// <returns></returns>
		//[NonAction]
		//public static async Task<User?> CreateUser(CreateUserReqDto dto, UserRepository _userRepository)
		//{
		//	var model = dto.ToModel();
		//	var res = await _userRepository.AddAsync(model, dto.PassWord);
		//	if (!res.Succeeded) return null;
		//	return model;
		//}
		//[NonAction]
		//public async Task CreateUser_(CreateUserReqDto dto)
		//{
		//	using var transaction = _context.Database.BeginTransaction();
		//	try
		//	{
		//		// 通用用户表
		//		var model = dto.ToModel();
		//		var res = await _userRepository.AddAsync(model, dto.PassWord);
		//		if (!res.Succeeded) throw new Exception("创建失败");
		//		foreach (var x in dto.Roles)
		//		{
		//			res = await _userRepository._userManager.AddToRoleAsync(model, x.ToString());
		//			if (!res.Succeeded) throw new Exception("创建失败，权限添加失败");
		//		}

		//		// 扩展信息用户表
		//		foreach (var x in dto.Roles)
		//		{
		//			switch (x)
		//			{
		//				case UserRole.Admin:
		//					{
		//						var extModel = dto.CreateAdminExt?.ToModel();
		//						if (extModel == null) throw new Exception("创建失败");
		//						extModel.UserId = model.Id;
		//						var result = await _adminRepository.AddAdminAsync(extModel);
		//						if (result == 0) throw new Exception("创建失败");
		//					}
		//					break;
		//				case UserRole.Academic:
		//					{
		//						var extModel = dto.CreateAcademicExt?.ToModel();
		//						if (extModel == null) throw new Exception("创建失败");
		//						extModel.UserId = model.Id;
		//						var result = await _academicRepository.AddAsync(extModel);
		//						if (result == 0) throw new Exception("创建失败");
		//					}
		//					break;
		//				case UserRole.Teacher:
		//					{
		//						var extModel = dto.CreateTeacherExt?.ToModel();
		//						if (extModel == null) throw new Exception("创建失败");
		//						extModel.UserId = model.Id;
		//						var result = await _teacherRepository.AddAsync(extModel);
		//						if (result == 0) throw new Exception("创建失败");
		//					}
		//					break;
		//				case UserRole.Student:
		//					{
		//						var extModel = dto.CreateStudentExt?.ToModel();
		//						if (extModel == null) throw new Exception("创建失败");
		//						extModel.UserId = model.Id;
		//						var result = await _studentRepository.AddAsync(extModel);
		//						if (result == 0) throw new Exception("创建失败");
		//					}
		//					break;
		//			}
		//		}

		//		await transaction.CommitAsync();
		//	}
		//	catch (Exception)
		//	{
		//		await transaction.RollbackAsync();
		//		throw;
		//	}
		//}

		
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<GetUserResDto>>> CreateUser([FromBody] CreateUserReqDto dto)
		{
			try
			{
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
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string id)
		{
			var res = await _userRepository.DeleteAsync(id);
			if (!res.Succeeded)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "删除失败", Data = null });
			return Ok(new ApiResponse<object> { Code = 1, Msg = "删除成功", Data = null });
		}


		/// <summary>
		/// 密码修改指定用户 超管
		/// </summary>
		/// <param name="reqDto"></param>
		/// <returns></returns>
		[HttpPut("change-password")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<object>>> ChangePassword(ChangePasswordReqDto reqDto)
		{

			var user = await _userRepository.GetByIdAsync(reqDto.UserId);
			if (user == null)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "未找到此用户", Data = null });

			if (reqDto.NewPassword != reqDto.ConfirmPassword)
				return Ok(new ApiResponse<object> { Code = 2, Msg = "新密码和确认密码不匹配。", Data = null });

			var result = await _userManager.ChangePasswordAsync(user, reqDto.CurrentPassword, reqDto.NewPassword);
			if (!result.Succeeded)
			{
				return Ok(new ApiResponse<object> { Code = 2, Msg = result.Errors.ToString(), Data = null });
			}

			return Ok(new ApiResponse<object> { Code = 1, Msg = "", Data = null });
		}

		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPut("update-user")]
		[Authorize(Roles = "Admin")]
		public async Task<OkObjectResult> Update([FromBody] UpdateProfileReqDto dto, string id)
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
