using StdMngMvc.Models;

namespace StdMngMvc.Data
{
    public class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            if (context.Students.Any())
            {
                return;
            }

            var students = new Student[]
            {

            };

            foreach (Student s in students)
            {
                context.Students.Add(s);
            }

            context.SaveChanges();
        }

        internal static async Task InitializeAsync(SchoolContext context)
        {
            throw new NotImplementedException();
        }
    }
}
