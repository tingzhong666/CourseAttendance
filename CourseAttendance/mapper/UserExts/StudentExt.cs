using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.UserExts
{
	public static class StudentExt
	{
		public static GetStudentResDto ToGetStudentResDto(this Student model, User UserModel)
		{
			return new GetStudentResDto
			{
				Id = model.UserId,
				GradeId = model.GradeId,
				Name = UserModel.Name,
				UserName = UserModel.UserName,
				Email = UserModel.Email,
				PhoneNumber = UserModel.PhoneNumber,
			};
		}
	}
}
