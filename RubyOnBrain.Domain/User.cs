using System.ComponentModel.DataAnnotations.Schema;

namespace RubyOnBrain.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}