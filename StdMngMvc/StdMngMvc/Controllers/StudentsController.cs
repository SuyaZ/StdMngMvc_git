using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using StdMngMvc.Data;
using StdMngMvc.Models;
using StdMngMvc.StdViewModels;
using Microsoft.Extensions.Configuration;

namespace StdMngMvc.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private IWebHostEnvironment hostingEnv;
        private string? sortOrder;
        public IConfiguration Configuration { get; }

        //public StudentsController(SchoolContext context, IWebHostEnvironment env)
        //{
        //    _context = context;
        //    this.hostingEnv = env;
        //}
        public StudentsController(SchoolContext context, IWebHostEnvironment env,
               IConfiguration configuration)
        {
            _context = context;
            this.hostingEnv = env;
            this.Configuration = configuration;
        }

        public String UploadStdImg()
        {
            var file = Request.Form.Files[0];

            var filenStorePath=hostingEnv.WebRootPath + $@"\UploadFiles\{file.Name}";

            using(FileStream fs=System.IO.File.Create(filenStorePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            return $"{file.Name}";
        }

        public String UpdateStdImg()
        {
            var file = Request.Form.Files[0];

            var filenStorePath = hostingEnv.WebRootPath + $@"\UploadFiles\{file.Name}";

            using (FileStream fs = System.IO.File.Create(filenStorePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            return $"{file.Name}";
        }


        // GET: Students
        //public async Task<IActionResult> Index(string sortOrder, string searchString)
        //{
        //    ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
        //    ViewData["NameSortParm"] = (sortOrder == "name" ? "name_desc" : "name");
        //    ViewData["BirthSortParm"] = (sortOrder == "birth" ? "birth_desc" : "birth");

        //    var students = from s in _context.Students select s;

        //    switch(sortOrder)
        //    {
        //        case "id_desc": 
        //            students = students.OrderByDescending(s => s.ID);
        //            break;

        //        case "name":
        //            students = students.OrderBy(s => s.Name);
        //            break;

        //        case "name_desc":
        //            students = students.OrderByDescending(s => s.Name);
        //            break;

        //        case "birth":
        //            students = students.OrderBy(s => s.Birth);
        //            break;

        //        case "birth_desc":
        //            students = students.OrderByDescending(s => s.Birth);
        //            break;

        //        default:
        //            students = students.OrderBy(s=>s.ID);
        //            break;

        //    }

        //    return View(await students.AsNoTracking().ToListAsync());
        //}
        // GET: Students

        //public IActionResult SaveStdGrades(int? id, string[] courseIds, string[] stdGrades,
        //    string currentFilter, string sortOrder, int? pageNumber)
        public string SaveStdGrades(int? id, string[] courseIds, string[] stdGrades)
        {
            if (id != null)
            {
                //得到当前学生的原始选课情况
                var enrollments = _context.Students.Include("Enrollments")
                                  .Single(s => s.ID == id).Enrollments;
                //得到学生的原始选课IDs
                var selCourseIds = new HashSet<int>(enrollments.Select(e => e.CourseID));

                for (int i = 0; i < courseIds.Length; i++)
                {
                    int curCourseID = Convert.ToInt32(courseIds[i]);
                    //如果原始选课IDs包含当前的课程ID
                    if (selCourseIds.Contains(curCourseID))
                    {
                        var curEnrollment = enrollments.Where(e => e.StudentID == id
                            && e.CourseID == curCourseID).FirstOrDefault();
                        //如果成绩不为空，修改选课记录
                        if (!String.IsNullOrWhiteSpace(stdGrades[i]))
                        {
                            curEnrollment.Grade = Convert.ToDecimal(stdGrades[i]);
                        }
                        //如果成绩为空，删除选课记录
                        else
                        {
                            _context.Enrollments.Remove(curEnrollment);
                        }
                    }
                    else
                    {
                        //如果原始选课IDs不包含当前的课程ID，但成绩不为空，添加选课记录
                        if (!String.IsNullOrWhiteSpace(stdGrades[i]))
                        {
                            Enrollment newEnrollment = new Enrollment()
                            {
                                StudentID = id.Value,
                                CourseID = curCourseID,
                                Grade = Convert.ToDecimal(stdGrades[i])

                            };

                            _context.Enrollments.Add(newEnrollment);
                        }
                    }
                }
                try
                {
                    _context.SaveChanges();
                    //return RedirectToAction("Index", "Students"
                    //, new { id, currentFilter, sortOrder, pageNumber, saveStdCourseSuc = true });
                    return "修改学生选课信息成功!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw ex;
                }
            }

            //return RedirectToAction("Error","Home");    
            return "修改学生选课信息错误!";
        }

        public void FillStdCourses(IEnumerable<Course> courses, IEnumerable<Enrollment> enrollments)
        {
            List<StdCourse> StdCourses = new List<StdCourse>();

            foreach (var c in courses)
            {
                StdCourse sc = new StdCourse();
                sc.CourseID = c.CourseID;
                sc.CourseName = c.CourseName;
                sc.DepartmentName = c.Department.DepartmentName;
                sc.CCredit = c.CCredit;

                if (enrollments != null)
                {
                    if (enrollments.FirstOrDefault(e => e.CourseID == c.CourseID) != null)
                    {
                        sc.Grade = enrollments.FirstOrDefault(e => e.CourseID == c.CourseID).Grade;
                    }
                    else
                    {
                        sc.Grade = null;
                    }
                }

                StdCourses.Add(sc);
            }

            ViewBag.StdCourses = StdCourses;
        }

        public async Task<IActionResult> Index(int? id,
            string currentFilter, string sortOrder,
            string searchString, int? pageNumber, bool saveStdCourseSuc = false)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ID_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewData["BirthSortParm"] = sortOrder == "Birth" ? "Birth_desc" : "Birth";

            if (saveStdCourseSuc)
                ViewData["Msg"] = "保存学生选课信息成功！";
            else
                ViewData["Msg"] = "";

            var students = from s in _context.Students select s;

            //如果选择课某个学生
            if (id != null)
            {
                ViewData["StdID"] = id.Value;
                //得到学生实例
                Student std = students.Single(s => s.ID == id);
                //得到所有课程，预加载系别信息
                List<Course> courses = _context.Courses.Include("Department").ToList();
                //预加载学生的选课记录，注意只加载当前学生的选课记录
                _context.Enrollments.Where(e => e.StudentID == id).Load();
                //填充返回的自定义ViewModel             
                FillStdCourses(courses, std.Enrollments);
            }

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString));
            }

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "ID";
            }

            bool descending = false;

            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder.Substring(0, sortOrder.Length - 5);
                descending = true;
            }

            if (descending)
            {
                students = students.OrderByDescending(e => EF.Property<object>(e, sortOrder));
            }
            else
            {
                students = students.OrderBy(e => EF.Property<object>(e, sortOrder));
            }
            //int pageSize = 7;
            int pageSize = Convert.ToInt32(Configuration.GetSection("ApplicationSetting:PageSize").Value);

            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(),
                                                     pageNumber ?? 1, pageSize));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Gender,Birth,Email,IDNums,ImageURL,Memo")] Student student)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // 输出模型验证错误信息
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Gender,Birth,Email,IDNums,ImageURL,Memo")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // 输出模型验证错误信息
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }




    }
}
