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
	}
}
