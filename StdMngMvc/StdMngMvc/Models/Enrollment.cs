using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StdMngMvc.Models
{
    public class Enrollment
    {
        [Display(Name = "学号")]
        public int StudentID { get; set; }

        [Display(Name = "课程号")]
        public int CourseID { get; set; }

        [Display(Name = "成绩"), Column(TypeName = "decimal(9,2)")]
        public decimal Grade { get; set; }

        [ForeignKey("CourseID")]
        public Course? Course { get; set; }

        [ForeignKey("StudentID")]
        public Student? Student { get; set; }


    }
}
