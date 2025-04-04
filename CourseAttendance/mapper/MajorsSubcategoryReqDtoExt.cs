using CourseAttendance.DtoModel.ReqDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class MajorsSubcategoryReqDtoExt
	{
		public static MajorsSubcategory ToModel(this MajorsSubcategoryReqDto dto)
		{
			return new MajorsSubcategory
			{
				//Id = dto.Id,
				MajorsCategoriesId = dto.MajorsCategoryId,
				Name = dto.Name,
			};
		}
	}
}
