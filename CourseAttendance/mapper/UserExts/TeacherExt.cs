using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class TeacherExt
	{
		public static async Task<GetTeacherResDto> ToGetTeacherResDto(this Teacher model, User UserModel, UserRepository userRepository)
		{
			var roles = await userRepository._userManager.GetRolesAsync(UserModel);
			return new GetTeacherResDto
			{
				Id = model.UserId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber,
				Roles = [.. roles ?? []],
				CourseIds = model.Courses.Select(x => x.Id).ToList(),
			};
		}
	}
}
