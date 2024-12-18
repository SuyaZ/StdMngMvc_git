using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StdMngMvc.Models
{
    //public enum Title
    //{
    //    教授, 副教授, 讲师, 助教
    //}
    //public class Teacher : Person
    //{
    //    [Display(Name = "工号"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public int ID { get; set; }
    //    [Required, Display(Name = "姓名"), StringLength(50)]
    //    public string Name { get; set; }
    //    [Required, Display(Name = "性别"), StringLength(2)]
    //    public string Gender { get; set; }
    //    [Required, Display(Name = "出生日期"), DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
    //        ApplyFormatInEditMode = true)]
    //    public DateTime Birth { get; set; }
    //    [Display(Name = "电子邮件"), StringLength(100)]
    //    [EmailAddress(ErrorMessage = "请输入正确的电子邮件地址！")]
    //    public string Email { get; set; }
    //    [Display(Name = "身份证号"), StringLength(18)]
    //    [RegularExpression(@"^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-8]|29|30|31)\d{3}[0-9Xx]$",
    //    ErrorMessage = "请输入正确的身份证号！")]
    //    public String IDNums { get; set; }
    //    [Display(Name = "年龄")]
    //    public int Age
    //    {
    //        get
    //        {
    //            return (int)DateTime.Now.Subtract(Birth).TotalDays / 365;
    //        }
    //    }
    //    [Display(Name = "登记照"), DataType(System.ComponentModel.DataAnnotations.DataType.Upload)]
    //    public String? ImageURL { get; set; }
    //    [Display(Name = "职称")]
    //    public Title Title { get; set; }
    //    public Department? Department { get; set; }
    //    public OfficeAssignment? OfficeAssignment { get; set; }
    //}

    public enum Title
    {
        教授, 副教授, 讲师, 助教
    }

    public class Teacher : Person
    {
        [Display(Name = "职称")]
        public Title Title { get; set; }
        public Department? Department { get; set; }
        public OfficeAssignment? OfficeAssignment { get; set; }
    }



}
