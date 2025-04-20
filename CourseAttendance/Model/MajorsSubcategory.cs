using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.Model
{
	public class MajorsSubcategory
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <summary>
		/// 大专业系
		/// </summary>
		[ForeignKey("MajorsCategory")]
		[Required]
		public int MajorsCategoriesId { get; set; }
		public virtual MajorsCategory MajorsCategory { get; set; }

		public virtual List<Grade> Grades { get; set; }

	}
}
