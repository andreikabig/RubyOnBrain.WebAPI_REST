namespace RubyOnBrain.API.Models
{
    public class TopicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }

        public int CourseId { get; set; }
    }
}
