using System.Text.Json.Serialization;

namespace RubyOnBrain.Domain
{
    public class UserCourse
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public int CourseId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }
        public int Rating { get; set; }
    }
}