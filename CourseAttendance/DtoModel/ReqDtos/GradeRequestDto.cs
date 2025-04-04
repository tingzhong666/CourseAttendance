using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class GradeRequestDto
	{
		[Required]
		public string Name { get; set; }
		/// <summary>
		/// 大专业系
		/// </summary>
		public int MajorsCategoriesId { get; set; }
		/// <summary>
		/// 专业
		/// </summary>
		public int MajorsSubcategoriesId { get; set; }
		/// <summary>
		/// 班级序号
		/// </summary>
		public int Num { get; set; }
		/// <summary>
		/// 入校年份 年级
		/// </summary>
		public int Year { get; set; }

		// 辅导员ID
		// public int CounselorId { get; set; }
	}
}
