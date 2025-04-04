using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;
using CourseAttendance.Repositories.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class AcademicExt
	{
		public static GetAcademicResDto ToGetAcademicResDto(this Academic model)
		{
			return new GetAcademicResDto
			{
			};
		}
		//public static async Task<GetAcademicResDto> ToGetAcademicResDto(this Academic model, User UserModel, UserRepository userRepository)
		//{
		//	var roles = await userRepository._userManager.GetRolesAsync(UserModel);
		//	return new GetAcademicResDto
		//	{
		//		Id = model.UserId,
		//		Name = UserModel.Name,
		//		UserName = UserModel.UserName,
		//		Email = UserModel.Email,
		//		PhoneNumber = UserModel.PhoneNumber,
		//		Roles = [.. roles ?? []],
		//	};
		//}
	}
}
