using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model
{
	/// <summary>
	/// 系统配置表，键值对表，记录不频繁更新的数据
	/// </summary>
	public class WebSystemConfig
	{
		[Key]
		[Required]
		[MaxLength(30)]
		public string Key { get; set; }

		[MaxLength(100)]
		public string Value { get; set; } = "";
	}
}
