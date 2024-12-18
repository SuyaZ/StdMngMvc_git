using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StdMngMvc.Models
{
    public class Department
    {
        [Display(Name = "系别号")]
        public int DepartmentID { get; set; }

        [Display(Name = "系别名称")]
        public String? DepartmentName { get; set; }

        [Required, Display(Name = "学费"), Range(1000, 10000, ErrorMessage = "学费必须在{1}-{2}之间")]
        public int Tuition {  get; set; }

        [Required, Display(Name = "成立日期"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }


        [Timestamp]
        public byte[]? RowVersion {  get; set; }


        [Display(Name = "系主任")]
        public int? TeacherID { get; set; }

        [Display(Name = "系主任"), ForeignKey("TeacherID")]
        public Teacher? Administrator { get; set; }
        public ICollection<Course>? Course {  get; set; }


    }
}
