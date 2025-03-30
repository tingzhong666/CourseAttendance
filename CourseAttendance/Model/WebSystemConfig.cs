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
		public string Key { get; set; }

		public string Value { get; set; } = "";
	}
}
