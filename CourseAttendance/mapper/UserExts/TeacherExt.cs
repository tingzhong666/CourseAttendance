using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class TeacherExt
	{
		public static GetTeacherResDto ToGetTeacherResDto(this Teacher model, User UserModel)
		{
			return new GetTeacherResDto
			{
				Id = model.UserId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber,
			};
		}
	}
}
