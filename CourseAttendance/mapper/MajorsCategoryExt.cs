using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class MajorsCategoryExt
	{
		public static MajorsCategoryResDto ToDto(this MajorsCategory model)
		{
			return new MajorsCategoryResDto
			{
				Id = model.Id,
				Name = model.Name,
			};
		}
	}
}
