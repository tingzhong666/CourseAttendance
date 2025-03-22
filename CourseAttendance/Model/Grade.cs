using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
    public class Grade
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
		// 大专业系
		// 专业
		// 班级序号
		// 入校年份

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		//[ForeignKey("Counselor")]
		//public int CounselorId { get; set; } // 外键，指向辅导员
		//public virtual Counselor Counselor { get; set; } // 对应的辅导员

		public virtual ICollection<Student> Students { get; set; } // 班级与学生的关系
	}
}
