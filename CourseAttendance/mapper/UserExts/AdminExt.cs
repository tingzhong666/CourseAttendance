using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class AdminExt
	{
		public static async Task<GetAdminResDto> ToGetAdminResDto(this Admin model, User UserModel, UserRepository userRepository)
		{
			var roles = await userRepository._userManager.GetRolesAsync(UserModel);
			return new GetAdminResDto
			{
				Id = model.UserId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber,
				Roles = [.. roles ?? []],
			};
		}
	}
}
