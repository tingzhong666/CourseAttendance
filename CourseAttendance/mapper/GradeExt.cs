using CourseAttendance.DtoModel.ResDtos;
using CourseAttendance.Model;

namespace CourseAttendance.mapper
{
	public static class GradeExt
	{
		public static GradeResponseDto ToResponseDto(this Grade model)
		{
			if (model == null) return null;

			return new GradeResponseDto
			{
				Id = model.Id,
				Name = model.Name,
				CreatedAt = model.CreatedAt,
				UpdatedAt = model.UpdatedAt,
				Num = model.Num,
				MajorsSubcategoriesId = model.MajorsSubcategoriesId,
				//MajorsCategoriesId = model.MajorsCategoriesId,
				Year = model.Year,
			};
		}
	}
}
