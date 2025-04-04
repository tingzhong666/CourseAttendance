using CourseAttendance.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class GradeResponseDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		///// <summary>
		///// 大专业系
		///// </summary>
		//public int MajorsCategoriesId { get; set; }
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

		//public List<GetStudentResDto> Students { get; set; } // 学
	}
}
