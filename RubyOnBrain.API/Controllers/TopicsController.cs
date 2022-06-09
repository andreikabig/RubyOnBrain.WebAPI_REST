using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubyOnBrain.API.Models;
using RubyOnBrain.API.Services;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private TopicService topicService;

        public TopicsController(TopicService topicService)
        {
            this.topicService = topicService;
        }

        // GET: api/topics
        [HttpGet]
        [Authorize(Roles = "admin")]
        public List<TopicDTO>? GetTopics()
        {
            var topics = topicService.GetTopics();

            return topics;
        }

        // GET: api/topics/{id}
        [HttpGet("{id:int}")]
        [Authorize]
        public ActionResult<TopicDTO?> GetTopic(int id)
        {
            TopicDTO topic = topicService.GetTopic(id, this.User);

            if (topic != null)
                return topic;
            return Problem($"We can't find the topic with id {id} or you don't have access to this one.");
        }

        // PUT: api/topics/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult UpdateTopic(int id, [FromBody] TopicDTO topic)
        {
            if (topic == null || topic.Id != id)
                return BadRequest();

            bool result = topicService.UpdateTopic(topic);

            if (result)
                return Ok(result);

            return Problem($"Something went wrong. We can't update the topic with id {id}.");
        }

        // DELETE: api/topics/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteTopic(int id)
        {
            bool result = topicService.DeleteTopic(id);

            if (result)
            {
                return Ok(result);
            }
            return Problem($"Something went wrong. We can't find topic with id {id}.");
        }

        // POST: api/topics
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult AddTopic(TopicDTO topic)
        {
            if (topic == null)
                return BadRequest();

            bool result = topicService.AddTopic(topic);
            if (result)
            {
                return Ok(result);
            }
            return Problem($"Something went wrong. We can't add your topic.");
        }

        // GET: api/topics/get-entries/{id}
        [HttpGet]
        [Authorize]
        [Route("get-entries/{id:int}")]
        public ActionResult<List<EntryDTO>> GetEntries(int id)
        {
            List<EntryDTO>? entries = topicService.GetEntries(id, this.User);

            if (entries != null)
            {
                return entries;
            }

            return Problem("Maybe entries are empty or you don't have access to this topic!");

        }

    }
}
