using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper.CreateUserReqDtoExts
{
	public static class CreateUserReqDtoExt
	{
		public static User ToModel(this CreateUserReqDto dto)
		{
			return new User
			{
				Email = dto.Email,
				PhoneNumber = dto.Phone,
				Name = dto.Name,
				UserName = dto.UserName
			};
		}
	}
}
