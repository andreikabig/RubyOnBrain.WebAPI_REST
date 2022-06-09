using Microsoft.EntityFrameworkCore;
using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;
using System.Security.Claims;

namespace RubyOnBrain.API.Services
{
    public class TopicService
    {
        private readonly DataContext db;

        public TopicService(DataContext db)
        {
            this.db = db;
        }

        public List<TopicDTO>? GetTopics()
        {
            return db?.Topics.Select(t => new TopicDTO() { Id = t.Id, Name = t.Name, Rating = t.Rating, CourseId = t.CourseId }).ToList();
        }

        public TopicDTO? GetTopic(int id, ClaimsPrincipal? user)
        {
            TopicDTO? topic = null;

            var findedTopic = db?.Topics.FirstOrDefault(t => t.Id == id);

            if (findedTopic != null)
            {
                if (user.IsInRole("admin"))
                {
                    if (findedTopic != null)
                        topic = new TopicDTO() { Id = findedTopic.Id, Name = findedTopic.Name, Rating = findedTopic.Rating, CourseId = findedTopic.CourseId };
                }
                else
                {
                    int userId = db.Users.FirstOrDefault(u => u.Email == user.Identity.Name).Id;
                    var uc = db.UserCourses.FirstOrDefault(uc => uc.UserId == userId && uc.CourseId == findedTopic.CourseId);

                    if (uc != null)
                    {
                        topic = new TopicDTO() { Id = findedTopic.Id, Name = findedTopic.Name, Rating = findedTopic.Rating, CourseId = findedTopic.CourseId };
                    }
                }
            }
            return topic;
        }

        public bool UpdateTopic(TopicDTO topic)
        {
            var findedTopic = db.Topics.FirstOrDefault(t => t.Id == topic.Id);
            var findedCourse = db.Courses.FirstOrDefault(c => c.Id == topic.CourseId);

            if (findedTopic != null && findedCourse != null)
            { 
                findedTopic.Name = topic.Name;
                findedTopic.Course = findedCourse;
                db.Topics.Update(findedTopic);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteTopic(int id)
        {
            var findedTopic = db.Topics.FirstOrDefault(t => t.Id == id);

            if (findedTopic != null)
            { 
                db.Topics.Remove(findedTopic);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddTopic(TopicDTO topic)
        {
            var course = db.Courses.FirstOrDefault(c => c.Id == topic.CourseId);

            if (course != null)
            {
                db.Topics.Add(new Topic() { Course = course, Name = topic.Name, Rating = 0 });
                db.SaveChanges();
                return true;
            }
            
            return false;
        }

        public List<EntryDTO>? GetEntries(int id, ClaimsPrincipal? user)
        {
            List<EntryDTO>? entries = null;

            var findedEntries = db?.Entries.Where(t => t.TopicId == id).ToList();
            var topic = GetTopic(id, user);

            if (topic != null)
            {
                if (user.IsInRole("admin"))
                {
                    entries = new List<EntryDTO>();
                    foreach (var entry in findedEntries)
                        entries.Add(new EntryDTO() { Id = entry.Id, EntryTypeId = entry.EntryTypeId, ImgName = entry.ImgName, Text = entry.Text, Title = entry.Title, TopicId = entry.TopicId, VideoName = entry.VideoName });
                }
                else
                {
                    int userId = db.Users.FirstOrDefault(u => u.Email == user.Identity.Name).Id;

                    if (topic != null)
                    {
                        var uc = db.UserCourses.FirstOrDefault(uc => uc.UserId == userId && uc.CourseId == topic.Id);

                        if (uc != null)
                        {
                            foreach (var entry in findedEntries)
                                entries.Add(new EntryDTO() { Id = entry.Id, EntryTypeId = entry.EntryTypeId, ImgName = entry.ImgName, Text = entry.Text, Title = entry.Title, TopicId = entry.TopicId, VideoName = entry.VideoName });
                        }
                    }

                }
            }
            return entries;
        }
    }
}
