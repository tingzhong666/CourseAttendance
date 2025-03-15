using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ResDtos
{
	public class GetStudentResDto : GetUserResDto
	{
		/// <summary>
		/// 班级
		/// </summary>
		[Required]
		public int GradeId { get; set; }

		/// <summary>
		/// 课程
		/// </summary>
		[Required]
		public List<int> courses { get; set; } = new List<int>();
	}
}
