namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceBatchReqQueryDto : ReqQueryDto
	{
		/// <summary>
		/// 老师id
		/// </summary>
		public List<string>? TeacherId { get; set; }
		/// <summary>
		/// 老师名 如果有老师id 优先老师id
		/// </summary>
		public string? TeacherName { get; set; }
		/// <summary>
		/// 时间范围
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 时间范围
		/// </summary>
		public DateTime? EndTime { get; set; }


		// 大专业
		public int? MajorsCategoryId { get; set; }
		// 小专业
		public int? MajorsSubcategoriesId { get; set; }
	}
}
