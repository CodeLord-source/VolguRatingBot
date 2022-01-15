using Microsoft.EntityFrameworkCore;
using RatingBot.Models;
using RatingBot.Models.Db;

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
            //this method add student to database
            await _context.Students.AddAsync(customObject);
            await _context.SaveChangesAsync();

            return customObject.Id;
        }

        public async Task<Student> GetStudentAsync(long id)
        {
            //this method finds the student by chat id and returns it
            var student = await _context.Students.FirstOrDefaultAsync(x => x.ChatId == id);

            return student;
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            //this method finds the student by id and returns it
            var student = await _context.Students.FindAsync(id);

            return student;
        }

        public async Task RemoveAsync(int id)
        {
            //this method delete the student from database
            var student = await _context.Students.FindAsync(id);

            _context.Remove(student);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student spaceObject)
        {
            //this method update student data
            _context.Entry(spaceObject).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}