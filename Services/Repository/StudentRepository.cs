using Microsoft.EntityFrameworkCore;
using RatingBot.Models;

namespace VolguRatingBot.Services.Repository.Interface
{
    public class StudentRepository : IRepository
    {
        public StudentRepository(StudentContext context)
        {
            _context = context;
        }

        private readonly StudentContext _context;

        public async Task<int> AddAsync(Student customObject)
        {
            await _context.Students.AddAsync(customObject);
            await _context.SaveChangesAsync();

            return customObject.Id;
        }

        public async Task<Student> GetStudentAsync(long id)
        {
            var student =await _context.Students.FirstOrDefaultAsync(x => x.ChatId == id);

            return student;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);

            return student;
        }

        public async Task RemoveAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student spaceObject)
        {
            _context.Entry(spaceObject).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}