﻿using CourseAttendance.Enums;

namespace CourseAttendance.DtoModel.ReqDtos
{
	public class AttendanceReqQueryDto : ReqQueryDto
	{
		/// <summary>
		/// 学生id
		/// </summary>
		public List<string>? StudentId { get; set; }
		/// <summary>
		/// 学生名 如果有学生id 优先学生id
		/// </summary>
		public string? StudentName { get; set; }
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

		// 考勤状态
		public AttendanceStatus? AttendanceStatus { get; set; }

		// 考勤批次id
		public int? BatchId { get; set; }
	}
}
