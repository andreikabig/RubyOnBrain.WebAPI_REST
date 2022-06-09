using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RubyOnBrain.Domain
{
    public class Entry
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? ImgName { get; set; }
        public string? VideoName { get; set; }
        public int EntryTypeId { get; set; }
        [ForeignKey("EntryTypeId")]
        public EntryType EntryType { get; set; }
        public int TopicId { get; set; }
        [ForeignKey("TopicId")]
        [JsonIgnore]
        public Topic Topic { get; set; }
    }
}