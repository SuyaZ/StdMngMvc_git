using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StdMngMvc.Models
{
    public class Course
    {
        [Display(Name = "课程号"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID {  get; set; }

        [Required, Display(Name = "课程名称"), StringLength(50)]
        public String? CourseName { get; set; }

        [Required, Display(Name = "学分"), Range(1, 5, ErrorMessage = "学分设置必须在{1}和{2}之间!")]
        public int CCredit {  get; set; }

        [Display(Name = "先修课")]
        public int? PreCourseID { get; set; }

        [Display(Name = "先修课"), ForeignKey("PreCourseID")]
        public Course? PreCourse { get; set; }
        public ICollection<Course>? FollowCourses { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }

        [Display(Name = "开课系别")]
        public int DepartmentID { get; set; }

        [Display(Name = "开课系别"), ForeignKey("DepartmentID")]
        public Department? Department { get; set; }






    }
}
