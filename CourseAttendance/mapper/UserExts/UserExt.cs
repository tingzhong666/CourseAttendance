﻿using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Enums;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class UserExt
	{
		public static async Task<GetUserResDto> ToGetUsersResDto(this User model, UserRepository userRepository)
		{
			var roles = await userRepository._userManager.GetRolesAsync(model);
			var roles_ = roles.ToList().Select(x => (UserRole)Enum.Parse(typeof(UserRole), x));
			return new GetUserResDto
			{
				Id = model.Id,
				Name = model.Name,
				UserName = model.UserName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				Roles = [.. roles_ ?? []],
			};
		}
	}
}
