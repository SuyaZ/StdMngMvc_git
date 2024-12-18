using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StdMngMvc.StdViewModels
{
    public class CourseStats
    {
        [Display(Name = "课程号")]
        public int CourseID { get; set; }
        [Display(Name = "选课人数")]
        public int Nums { get; set; }
        [Display(Name = "平均成绩")]
        public int AvgGrade { get; set; }
    }
}
