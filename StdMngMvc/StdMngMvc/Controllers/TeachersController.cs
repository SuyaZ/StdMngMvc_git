using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StdMngMvc.Data;
using StdMngMvc.Models;

namespace StdMngMvc.Controllers
{
    public class TeachersController : Controller
    {
        private readonly SchoolContext _context;
        private IWebHostEnvironment hostingEnv;
        private string? sortOrder;

        public TeachersController(SchoolContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.hostingEnv = env;
        }

        public String UploadTchImg()
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

        public String UpdateTchImg()
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



        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teachers
                           .Include(t => t.OfficeAssignment)
                           .ToListAsync());
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Gender,Birth,Email,IDNums,ImageURL,Title,OfficeAssignment")] Teacher teacher)
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
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                           .Include(t => t.OfficeAssignment)
                           .FirstOrDefaultAsync(t => t.ID == id);

            if (teacher == null)
            {
                return NotFound();
            }

            //ViewData["WebUrl"] = Configuration.GetSection("ApplicationSetting:WebUrl").Value;
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var teacherUpdate = _context.Teachers.Include(t => t.OfficeAssignment)
                      .FirstOrDefault(t => t.ID == id);

            if (await TryUpdateModelAsync<Teacher>(teacherUpdate, "",
                i => i.Name, i => i.Gender, i => i.Birth, i => i.Email,
                i => i.IDNums, i => i.ImageURL, i => i.Title, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(teacherUpdate.OfficeAssignment?.Location))
                {
                    teacherUpdate.OfficeAssignment = null;
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes.");
                }
                return RedirectToAction(nameof(Index));
            }

            return View(teacherUpdate);
        }


        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.ID == id);
        }
    }
}
