namespace RubyOnBrain.API.Models
{
    public class EntryDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? ImgName { get; set; }
        public string? VideoName { get; set; }
        public int EntryTypeId { get; set; }
        public int TopicId { get; set; }
    }
}
