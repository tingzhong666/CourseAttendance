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
		public async Task<IdentityResult> UpdateAsync(UpdateProfileReqDto user, string Id)
		{
			var userModel = await _userRepository._userManager.FindByIdAsync(Id);
			if (userModel == null)
				return IdentityResult.Failed([new IdentityError { Description = "用户Id失败" }]);

			var model = user.ToUserModel();
			model.Id = userModel.Id;
			var result = await _userRepository.UpdateAsync(model);
			return result;
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

		public async Task<GetUserResDto?> GetInfoById(string id)
		{
			var model = await _userRepository.GetByIdAsync(id);
			if (model == null) return null;
			var dto = await ModelToDtoAsync(model);
			return dto;
		}
	}
}
