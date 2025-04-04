using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class MajorsSubcategoryExt
	{
		public static MajorsSubcategoryResDto ToDto(this MajorsSubcategory model)
		{
			return new MajorsSubcategoryResDto
			{
				Name = model.Name,
				Id = model.Id,
				MajorsCategoriesId = model.MajorsCategoriesId,
			};
		}
	}
}
