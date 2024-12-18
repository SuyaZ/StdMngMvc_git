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
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;
        

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var department = await _context.Departments
            //    .Include(d => d.Administrator)
            //    .FirstOrDefaultAsync(m => m.DepartmentID == id);  ??Fromsql与FromsqlRaw
            string query = "SELECT * FROM Department WHERE DepartmentID = {0}";

            var department = await _context.Departments
                .FromSqlRaw(query, id)
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync();


            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,DepartmentName,Tuition,StartDate,TeacherID")] Department department)
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
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name", department.TeacherID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name", department.TeacherID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,DepartmentName,Tuition,StartDate,TeacherID")] Department department)
        //{
        //    if (id != department.DepartmentID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(department);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DepartmentExists(department.DepartmentID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name", department.TeacherID);
        //    return View(department);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments.Include(i => i.Administrator).
                                     FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty, "无法保存修改，记录已经被其它用户删除！");
                ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name", deletedDepartment.TeacherID);
                return View(deletedDepartment);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate, "",
                s => s.DepartmentName, s => s.StartDate, s => s.Tuition, s => s.TeacherID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "无法保存修改，记录已经被其它用户删除！");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.DepartmentName != clientValues.DepartmentName)
                        {
                            ModelState.AddModelError("DepartmentName", $"当前值: {databaseValues.DepartmentName}");
                        }
                        if (databaseValues.Tuition != clientValues.Tuition)
                        {
                            ModelState.AddModelError("Tuition", $"当前值: {databaseValues.Tuition:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"当前值: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.TeacherID != clientValues.TeacherID)
                        {
                            Teacher databaseInstructor = await _context.Teachers.FirstOrDefaultAsync(i => i.ID == databaseValues.TeacherID);
                            ModelState.AddModelError("TeacherID", $"当前值: {databaseInstructor?.Name}");
                        }

                        ModelState.AddModelError(string.Empty, "该记录以及被其它用户修改，" +
                                                "如果需要保持该修改值，请返回列表。如果希望继续修改，请点击保存按钮");
                        departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }

            ViewData["TeacherID"] = new SelectList(_context.Teachers, "ID", "Name", departmentToUpdate.TeacherID);
            return View(departmentToUpdate);
        }



        // GET: Departments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _context.Departments
        //        .Include(d => d.Administrator)
        //        .FirstOrDefaultAsync(m => m.DepartmentID == id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(department);
        //}

        // POST: Departments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var department = await _context.Departments.FindAsync(id);
        //    if (department != null)
        //    {
        //        _context.Departments.Remove(department);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}



        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? conCurrencyError)
        {
            if(id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.Include(d=>d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m=>(m.DepartmentID==id));

            if (department == null)
            {
                if(conCurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }

            if (conCurrencyError.GetValueOrDefault())
            {
                ViewData["concurrencyErrorMessage"] = "删除记录已经被其他用户更改" + "你可以选择继续删除, 或者选择返回列表!";
                
            }

            return View(department);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                if (await _context.Departments.AnyAsync(m => m.DepartmentID
                                                         == department.DepartmentID))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new
                {
                    concurrencyError = true,
                    id = department.DepartmentID
                });
            }
        }


        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
