using Microsoft.EntityFrameworkCore;
using StdMngMvc.Models;


namespace StdMngMvc.Data
{
    //public class SchoolContext : DbContext
    //{
    //    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
    //    {

    //    }

    //    public DbSet<Department>? Departments { get; set; }
    //    public DbSet<Student>? Students { get; set; }
    //    public DbSet<Course>? Courses { get; set; }
    //    public DbSet<Enrollment>? Enrollments { get; set; }
    //    public DbSet<Teacher>? Teachers { get; set; }
    //    public DbSet<OfficeAssignment>? officeAssignments { get; set; }

    //    public DbSet<Person> People { get; set; }


    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Department>().ToTable("Department");
    //        modelBuilder.Entity<Student>().ToTable("Student");
    //        modelBuilder.Entity<Course>().ToTable("Course");
    //        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
    //        modelBuilder.Entity<Teacher>().ToTable("Teacher");
    //        modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");

    //        modelBuilder.Entity<Enrollment>().HasKey(c => new {c.StudentID, c.CourseID});
    //    }


    //}

    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Teacher>().ToTable("Teacher");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<Person>().ToTable("Person");
            //设置Enrollment表的主键
            modelBuilder.Entity<Enrollment>()
                .HasKey(c => new { c.StudentID, c.CourseID });
        }
    }




}
