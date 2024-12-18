using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StdMngMvc.Models
{
    public abstract class Person
    {
        [Display(Name = "编号"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        [Required, Display(Name = "姓名"), StringLength(50)]
        public String Name { get; set; }
        [Required, Display(Name = "性别"), StringLength(2)]
        public String Gender { get; set; }
        [Required, Display(Name = "出生日期"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birth { get; set; }
        [Display(Name = "电子邮箱"), StringLength(100)]
        [EmailAddress(ErrorMessage = "请输入正确的电子邮箱地址！")]
        public String Email { get; set; }
        [Display(Name = "身份证号码"), StringLength(18)]
        [RegularExpression(@"^[1-9]\d{5}(18|19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$",
        ErrorMessage = "请输入正确的身份证号！")]
        public String IDNums { get; set; }
        [Display(Name = "年龄")]
        public int Age
        {
            get
            {
                return (int)DateTime.Now.Subtract(Birth).TotalDays / 365;
            }
        }
        [Display(Name = "登记照"), DataType(DataType.Upload)]
        public String? ImageURL { get; set; }
    }
}
