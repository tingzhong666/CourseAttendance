using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class AcademicExt
	{
		public static GetAcademicResDto ToGetAcademicResDto(this Academic model, User UserModel)
		{
			return new GetAcademicResDto
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
