using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class UserExt
	{
		public static GetUserResDto ToGetUsersResDto(this User model)
		{
			return new GetUserResDto
			{
				Id = model.Id,
				Name = model.Name,
				UserName = model.UserName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber
			};
		}
	}
}
