using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Diagnostics;
using CourseAttendance.Enums;
using Microsoft.AspNetCore.Identity;

namespace CourseAttendance.Model.Users
{
	public class User : IdentityUser
	{

		//[Required]
		//public string Password { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		[Required]
		public required string Name { get; set; }

		//[Required]
		//public UserRole Role { get; set; }

		//public string Email { get; set; }
		//public string? Phone { get; set; }
		//public DateTime CreatedAt { get; set; } = DateTime.Now;
		//public DateTime UpdatedAt { get; set; } = DateTime.Now;

		[Required]
		public override string UserName { get; set; }
	}
}
