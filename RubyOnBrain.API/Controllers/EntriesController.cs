using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubyOnBrain.API.Models;
using RubyOnBrain.API.Services;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EntriesController : ControllerBase
    {
        private EntryService entryService;

        public EntriesController(EntryService entryService)
        {
            this.entryService = entryService;
        }

        // GET: api/entries
        [HttpGet]
        [Authorize(Roles = "admin")]
        public List<EntryDTO> GetEntries()
        {
            return entryService.GetEntries();
        }

        // GET: api/entries/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult<EntryDTO> GetEntry(int id)
        {
            var entry = entryService.GetEntry(id);

            if (entry != null)
                return entry;
            return Problem($"We can't find an entry with id {id}.");
        }

        // PUT: api/entries/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult<EntryDTO> UpdateEntry(int id, [FromBody] EntryDTO entry)
        {
            if (entry == null || entry.Id != id)
                return BadRequest();
            bool result = entryService.UpdateEntry(entry);

            if (result)
                return Ok(result);

            return Problem($"Something went wrong. We can't update an entry with id {id}.");
        }

        // DELETE: api/entries/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteEntry(int id)
        {
            bool result = entryService.DeleteEntry(id);

            if (result)
            {
                return Ok(result);
            }
            return Problem($"Something went wrong. We can't delete entry with id {id}.");
        }

        // POST: api/entries
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult AddEntry(EntryDTO entry)
        {
            if (entry == null)
                return BadRequest();

            bool result = entryService.AddEntry(entry);

            if (result)
            {
                return Ok(result);
            }
            return Problem($"Something went wrong. We can't add your entry.");
        }
    }
}