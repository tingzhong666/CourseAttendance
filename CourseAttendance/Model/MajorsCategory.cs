using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAttendance.Model
{
	public class MajorsCategory
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(10)]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		

		public virtual List<MajorsSubcategory> MajorsSubcategories { get; set; }
	}
}
