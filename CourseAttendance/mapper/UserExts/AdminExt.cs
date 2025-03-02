using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class AdminExt
	{
		public static GetAdminResDto ToGetAdminResDto(this Admin model, User UserModel)
		{
			return new GetAdminResDto
			{
				Id = model.UserId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber
			};
		}
	}
}
