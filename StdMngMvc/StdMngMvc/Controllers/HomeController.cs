using System.Data.Common;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StdMngMvc.Data;
using StdMngMvc.Models;
using StdMngMvc.StdViewModels;

namespace StdMngMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolContext _context;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Privacy()
        {
            List<CourseStats> groups = new List<CourseStats>();

            var conn = _context.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    string query = "select CourseID,count(CourseID) as Nums, " +
                        "Convert(int,avg(Grade)) as AvgGrade from Enrollment " +
                        "group by CourseID ";

                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new CourseStats
                            {
                                CourseID = reader.GetInt32(0),
                                Nums = reader.GetInt32(1),
                                AvgGrade = reader.GetInt32(2)
                            };
                            groups.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return View(groups);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
