using Microsoft.EntityFrameworkCore;

namespace RatingBot.Models
{
    public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
