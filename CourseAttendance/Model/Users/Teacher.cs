using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseAttendance.Model.Users
{
    public class Teacher
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; } // 外键，作为主键

        public virtual User User { get; set; } // 对应的用户信息
    }
}
