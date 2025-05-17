using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseAttendance.Model.Users;

namespace CourseAttendance.Model
{
	public class Grade
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(10)]
		public string? Name { get; set; }
		/// <summary>
		/// 专业
		/// </summary>
		[ForeignKey("MajorsSubcategory")]
		[Required]
		public int MajorsSubcategoriesId { get; set; }
		/// <summary>
		/// 班级序号
		/// </summary>
		[Required]
		public int Num { get; set; }
		/// <summary>
		/// 入校年份 年级
		/// </summary>
		[Required]
		public int Year { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		//[ForeignKey("Counselor")]
		//public int CounselorId { get; set; } // 外键，指向辅导员
		//public virtual Counselor Counselor { get; set; } // 对应的辅导员

		public virtual ICollection<Student> Students { get; set; } // 班级与学生的关系
		public virtual MajorsSubcategory MajorsSubcategory { get; set; }
	}
}
