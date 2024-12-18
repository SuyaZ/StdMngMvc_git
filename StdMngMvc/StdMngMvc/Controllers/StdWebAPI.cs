using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StdMngMvc.Data;
using StdMngMvc.Models;

namespace StdMngMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class StdWebAPI : ControllerBase
    {
        private readonly SchoolContext _context;
        private IWebHostEnvironment hostingEnv;

        //private IHostingEnvironment hostingEnv;

        public StdWebAPI(SchoolContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.hostingEnv = env;
        }

        // GET: api/StdWebAPI
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        //{
        //    return await _context.Students.ToListAsync();
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> 
            GetStudents(string stdName="", int pageSize=5, int pageIndex=1)
        {
            var students = from s in _context.Students select s;

            //查找操作不分页
            if (!String.IsNullOrEmpty(stdName))
            {
                students = students.Where(s => s.Name.Contains(stdName));
                return await students.ToListAsync();
            }
            return await students.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            //return await _context.Students.ToListAsync();
        }


        //返回学生总数
        [HttpGet("GetStdCounts")]
        public int GetStdCounts()
        {
            return _context.Students.Count();
        }


        // GET: api/StdWebAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/StdWebAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // POST: api/StdWebAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/StdWebAPI/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudent(int id)
        //{
        //    var student = await _context.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Students.Remove(student);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return student;
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }



        [HttpPost("StdImgFileUpload")]
        public async Task<HttpResponseMessage> StdImgFileUpload(IFormFile stdImgfile)
        {
            if (stdImgfile != null)
            {
                try
                {   //得到存储的绝对路径
                    var filenStorePath = hostingEnv.WebRootPath + $@"\UploadFiles\{stdImgfile.FileName}";

                    using (FileStream fs = System.IO.File.Create(filenStorePath))
                    {
                        stdImgfile.CopyTo(fs);
                        await fs.FlushAsync();
                    }

                    //返回结果
                    return new HttpResponseMessage() { Content = new StringContent("上传学生登记照成功！") };
                    //return "上传学生登记照成功！";
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage() { Content = new StringContent(ex.Message) };
                }
            }
            else
            {

                return new HttpResponseMessage() { Content = new StringContent("图片文件为空！") };
                //return "图片文件为空！";
            }
        }


    }
}
