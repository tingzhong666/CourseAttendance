using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class MajorsCategoryReqDtoExt
	{
		public static MajorsCategory ToModel(this MajorsCategoryReqDto dto)
		{
			return new MajorsCategory
			{
				//Id = dto.Id,
				Name = dto.Name,
			};
		}
	}
}
