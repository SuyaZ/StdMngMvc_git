using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StdMngMvc.Models;

namespace StdMngMvc.StdViewModels
{
    public class StdCourse
    {
        //课程号"
        public int CourseID { get; set; }
        //课程名称"
        public string CourseName { get; set; }
        //"学分"
        public int CCredit { get; set; }
        //开课系别"
        public string DepartmentName { get; set; }
        // "成绩"
        public decimal? Grade { get; set; }
    }
}
