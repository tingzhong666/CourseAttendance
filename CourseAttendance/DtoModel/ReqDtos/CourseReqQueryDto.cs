using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class CourseReqQueryDto : ReqQueryDto
	{
		/// <summary>
		/// 学生
		/// </summary>
		public List<string> studentIds { get; set; } = [];
	}
}