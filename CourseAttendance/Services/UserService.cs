using CourseAttendance.AppDataContext;
using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Enums;
using CourseAttendance.mapper.CreateUserReqDtoExts;
using CourseAttendance.mapper.UpdateProfileReqDtoExtends;
using CourseAttendance.mapper.UserExts;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseAttendance.Services
{
	public class UserService
	{
		private readonly AppDBContext _context;
		private readonly UserRepository _userRepository;
		private readonly AcademicRepository _academicRepository;
		private readonly AdminRepository _adminRepository;
		private readonly StudentRepository _studentRepository;
		private readonly TeacherRepository _teacherRepository;

		public UserService(AppDBContext context, AcademicRepository academicRepository, AdminRepository adminRepository, StudentRepository studentRepository, TeacherRepository teacherRepository, UserRepository userRepository)
		{
			_context = context;
			_academicRepository = academicRepository;
			_adminRepository = adminRepository;
			_studentRepository = studentRepository;
			_teacherRepository = teacherRepository;
			_userRepository = userRepository;
		}

		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <param name="user"></param>
		/// <param name="Id"></param>
		/// <returns></returns>
		public async Task<string?> UpdateAsync(UpdateProfileReqDto user, string Id)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var userModel = await _userRepository._userManager.FindByIdAsync(Id);
				if (userModel == null)
					throw new Exception("用户获取失败");

				// 通用信息更新
				var model = user.ToUserModel();
				model.Id = userModel.Id;
				var result = await _userRepository.UpdateAsync(model);
				if (result != IdentityResult.Success)
					throw new Exception("用户信息更新失败");

				// 密码这里不更新 单独抽离一个方法

				// 权限更新，并删除老权限的扩展信息
				if (user.roles.Count < 1)
					throw new Exception("用户信息更新失败");
				var rolesOld = await _userRepository._userManager.GetRolesAsync(model);
				foreach (var roleOld in rolesOld)
				{
					var roleOld_ = Enum.Parse<UserRole>(roleOld);
					var res = 0;
					switch (roleOld_)
					{
						case UserRole.Admin:
							res = await _adminRepository.DeleteAdminAsync(model.Id);
							break;
						case UserRole.Academic:
							res = await _academicRepository.DeleteAsync(model.Id);
							break;
						case UserRole.Teacher:
							res = await _teacherRepository.DeleteAsync(model.Id);
							break;
						case UserRole.Student:
							res = await _studentRepository.DeleteAsync(model.Id);
							break;
					}
					if (res == 0) throw new Exception("用户信息更新失败");
				}
				result = await _userRepository._userManager.RemoveFromRolesAsync(model, rolesOld);
				if (result != IdentityResult.Success)
					throw new Exception("用户信息更新失败");
				result = await _userRepository._userManager.AddToRolesAsync(model, user.roles.Select(x => x.ToString()));
				if (result != IdentityResult.Success)
					throw new Exception("用户信息更新失败");

				// 扩展信息，新增
				foreach (var role in user.roles)
				{
					var res = 0;
					switch (role)
					{
						case UserRole.Admin:
							if(user.CreateAdminExt == null) throw new Exception("用户信息更新失败");
							res = await _adminRepository.AddAdminAsync(user.CreateAdminExt.ToModel());
							break;
						case UserRole.Academic:
							if(user.CreateAcademicExt == null) throw new Exception("用户信息更新失败");
							res = await _academicRepository.AddAsync(user.CreateAcademicExt.ToModel());
							break;
						case UserRole.Teacher:
							if(user.CreateTeacherExt == null) throw new Exception("用户信息更新失败");
							res = await _teacherRepository.AddAsync(user.CreateTeacherExt.ToModel());
							break;
						case UserRole.Student:
							if(user.CreateStudentExt == null) throw new Exception("用户信息更新失败");
							res = await _studentRepository.AddAsync(user.CreateStudentExt.ToModel());
							break;
					}
					if (res == 0) throw new Exception("用户信息更新失败");
				}

				await transaction.CommitAsync();
				return null;
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return err.Message;
			}
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="Id"></param>
		/// <param name="pw"></param>
		/// <returns></returns>
		public async Task<string?> UpdatePW(string Id,string pw)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var userModel = await _userRepository._userManager.FindByIdAsync(Id);
				if (userModel == null)
					throw new Exception("用户获取失败");

				var res=await _userRepository._userManager.ChangePasswordAsync(userModel, pw, pw);
				if(res != IdentityResult.Success)
					throw new Exception("密码修改失败");


				await transaction.CommitAsync();
				return null;
			}
			catch (Exception err)
			{
				await transaction.RollbackAsync();
				return err.Message;
			}
		}


		/// <summary>
		/// Model转响应dto
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<GetUserResDto> ModelToDtoAsync(User model)
		{
			var dto = await model.ToGetUsersResDto(_userRepository);
			foreach (var x in dto.Roles)
			{
				switch (x)
				{
					case UserRole.Admin:
						{
							var extModel = await _adminRepository.GetAdminByIdAsync(dto.Id);
							dto.GetAdminExt = extModel.ToGetAdminResDto();
						}
						break;
					case UserRole.Academic:
						{
							var extModel = await _academicRepository.GetByIdAsync(dto.Id);
							dto.GetAcademicExt = extModel.ToGetAcademicResDto();
						}
						break;
					case UserRole.Teacher:
						{
							var extModel = await _teacherRepository.GetByIdAsync(dto.Id);
							dto.GetTeacherExt = extModel.ToGetTeacherResDto();
						}
						break;
					case UserRole.Student:
						{
							var extModel = await _studentRepository.GetByIdAsync(dto.Id);
							dto.GetStudentExt = extModel.ToGetStudentResDto();
						}
						break;
				}
			}

			return dto;
		}

		/// <summary>
		/// 创建用户
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		public async Task<string> CreateUserAsync(CreateUserReqDto dto)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				// 通用用户表
				var model = dto.ToModel();
				var res = await _userRepository.AddAsync(model, dto.PassWord);
				if (!res.Succeeded) throw new Exception("创建失败");
				foreach (var x in dto.Roles)
				{
					res = await _userRepository._userManager.AddToRoleAsync(model, x.ToString());
					if (!res.Succeeded) throw new Exception("创建失败，权限添加失败");
				}

				// 扩展信息用户表
				foreach (var x in dto.Roles)
				{
					switch (x)
					{
						case UserRole.Admin:
							{
								var extModel = dto.CreateAdminExt?.ToModel();
								if (extModel == null) throw new Exception("创建失败");
								extModel.UserId = model.Id;
								var result = await _adminRepository.AddAdminAsync(extModel);
								if (result == 0) throw new Exception("创建失败");
							}
							break;
						case UserRole.Academic:
							{
								var extModel = dto.CreateAcademicExt?.ToModel();
								if (extModel == null) throw new Exception("创建失败");
								extModel.UserId = model.Id;
								var result = await _academicRepository.AddAsync(extModel);
								if (result == 0) throw new Exception("创建失败");
							}
							break;
						case UserRole.Teacher:
							{
								var extModel = dto.CreateTeacherExt?.ToModel();
								if (extModel == null) throw new Exception("创建失败");
								extModel.UserId = model.Id;
								var result = await _teacherRepository.AddAsync(extModel);
								if (result == 0) throw new Exception("创建失败");
							}
							break;
						case UserRole.Student:
							{
								var extModel = dto.CreateStudentExt?.ToModel();
								if (extModel == null) throw new Exception("创建失败");
								extModel.UserId = model.Id;
								var result = await _studentRepository.AddAsync(extModel);
								if (result == 0) throw new Exception("创建失败");
							}
							break;
					}
				}

				await transaction.CommitAsync();
				return model.Id;
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		/// <summary>
		/// 获取单个通过id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<GetUserResDto?> GetInfoById(string id)
		{
			var model = await _userRepository.GetByIdAsync(id);
			if (model == null) return null;
			var dto = await ModelToDtoAsync(model);
			return dto;
		}
	}
}
