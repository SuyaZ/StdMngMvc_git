using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StdMngMvc.Models
{
    public class OfficeAssignment
    {
        [Key]
        public int TeacherID { get; set; }
        [Display(Name = "办公室"), StringLength(50)]
        public String? Location { get; set; }

        [ForeignKey("TeacherID")]
        public Teacher? Teacher { get; set; }
    }
}
