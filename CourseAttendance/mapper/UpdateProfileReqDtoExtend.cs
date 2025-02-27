using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model.Users;

namespace CourseAttendance.mapper
{
	public static class UpdateProfileReqDtoExtend
	{
		public static User ToUserModel(this UpdateProfileReqDto user)
		{
			return new User
			{
				Email = user.Email,
				PhoneNumber = user.Phone,
				Name = user.Name,
				Id = user.Id
			};
		}
		public static Academic ToAcademicModel(this UpdateProfileReqDto user)
		{
			return new Academic
			{
				UserId = user.Id
			};
		}
	}
}
