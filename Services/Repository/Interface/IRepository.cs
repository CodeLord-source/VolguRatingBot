using RatingBot.Models; 

namespace VolguRatingBot.Services.Repository.Interface
{
    public interface IRepository
    {
        public Task<Student> GetStudentAsync(long id);
          
        public Task<Student> GetStudentAsync(int id);

        public Task<int> AddAsync(Student customObject);

        public Task RemoveAsync(int id);

        public Task UpdateAsync(Student spaceObject);
    }
}
