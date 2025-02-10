using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Diagnostics;
using CourseAttendance.Enums;

namespace CourseAttendance.Model
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public UserRole Role { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public int? GradeId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		public virtual Grade Grade { get; set; }
	}
}
