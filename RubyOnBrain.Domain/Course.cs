using System.ComponentModel.DataAnnotations.Schema;


namespace RubyOnBrain.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        [ForeignKey("ProgLangId")]
        public ProgLang ProgLang { get; set; }
        public List<Topic> Topics { get; set; } = new List<Topic>();

        public List<User> Users { get; set; } = new List<User>();

        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}
