using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model
{
	public class MajorsCategory
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual List<MajorsSubcategory> MajorsSubcategories { get; set; }
	}
}
