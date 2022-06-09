using Microsoft.EntityFrameworkCore;
using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;

namespace RubyOnBrain.API.Services
{
    public class UserService
    {
        private readonly DataContext db;
        public UserService(DataContext db)
        { 
            this.db = db;
        }

        // Method for retrieving current user data
        public UserDTO? GetData(string? name)
        {
            var findedUser = db?.Users.Include(r => r.Role).FirstOrDefault(u => u.Email == name);

            if (findedUser != null)
            {
                return ConvertData(findedUser);
            }
            else 
                return null;
        }

        // Method for updating the data of the current user
        public bool UpdateUser(UserDTO user)
        {
            var findedUser = db?.Users.Include(r => r.Role).FirstOrDefault(u => u.Email == user.Email);

            if (findedUser != null)
            { 
                findedUser.Email = user.Email;
                findedUser.PhoneNumber = user.PhoneNumber;
                findedUser.FirstName = user.FirstName;
                findedUser.LastName = user.LastName;
                findedUser.Password = user.Password;

                db?.Users.Update(findedUser);
                db?.SaveChanges();

                return true;
            }
            return false;
        }

        // The method of obtaining the courses available to the user
        public List<CourseDTO>? GetCourses(string? name)
        {
            var userId = db?.Users.FirstOrDefault(u => u.Email == name)?.Id;
            var findedCourses = db?.UserCourses.Include(u => u.Course).ThenInclude(c => c.ProgLang).Where(c => c.UserId == userId).Select(us => us.Course).ToList();

            return ConvertData(findedCourses);
        }

        public UserDTO ConvertData(User dataUser)
        {
            return new UserDTO()
            {
                Id = dataUser.Id,
                FirstName = dataUser.FirstName,
                LastName = dataUser.LastName,
                Email = dataUser.Email,
                Password = dataUser.Password,
                Role = dataUser.Role.Name,
                PhoneNumber = dataUser.PhoneNumber
            };
        }

        private List<CourseDTO>? ConvertData(List<Course>? dataCourses)
        {
            List<CourseDTO> courses = new List<CourseDTO>();

            if (dataCourses != null)
            {
                foreach (var course in dataCourses)
                {
                    courses.Add(new CourseDTO()
                    {
                        Id = course.Id,
                        Description = course.Description,
                        Name = course.Name,
                        ProgLang = course.ProgLang.Name,
                        Rating = course.Rating
                    });
                }

                return courses;
            }
            else
                return null;
        }


    }
}
