using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseRequestDto
	{
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 上课时间的哦
		/// </summary>
		[Required]
		public List<CourseTimeReqDto> CourseTimes { get; set; }

		[Required]
		public string Location { get; set; }

		[Required]
		public string TeacherId { get; set; }

		// 小专业
		[Required]
		public int MajorsSubcategoryId { get; set; }

	}
}
