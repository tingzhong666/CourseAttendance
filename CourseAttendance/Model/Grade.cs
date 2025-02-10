using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model
{
	public class Grade
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int TeacherId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		public virtual User Teacher { get; set; }
	}
}
