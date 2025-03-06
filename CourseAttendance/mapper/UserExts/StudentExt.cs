using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class StudentExt
	{
		public static async Task<GetStudentResDto> ToGetStudentResDto(this Student model, User UserModel, UserRepository userRepository)
		{
			var roles = await userRepository._userManager.GetRolesAsync(UserModel);
			return new GetStudentResDto
			{
				Id = model.UserId,
				GradeId = model.GradeId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber,
				Roles = [.. roles ?? []],
			};
		}
	}
}
