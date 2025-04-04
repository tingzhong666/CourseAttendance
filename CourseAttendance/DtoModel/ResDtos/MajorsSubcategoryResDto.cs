namespace CourseAttendance.DtoModel.ResDtos
{
	public class MajorsSubcategoryResDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// 大专业系
		/// </summary>
		public int MajorsCategoriesId { get; set; }
	}
}
