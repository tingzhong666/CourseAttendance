using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model.Users
{
	public class Admin
	{
		[Key]
		[ForeignKey("User")]
		public string UserId { get; set; } 

		public virtual User User { get; set; } 
	}
}
