using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubyOnBrain.API.Models;
using RubyOnBrain.API.Services;
using RubyOnBrain.Domain;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RecsystemController : ControllerBase
    {
        private RecommendationService recService; 
        public RecsystemController(RecommendationService recService)
        { 
            this.recService = recService;
        }

        // GET: api/recsystem/get-files
        [HttpGet]
        [Authorize(Roles = "admin")]
        [ActionName("get-files")]
        public ActionResult<List<string>> GetFileNames()
        {
            List<string>? fileNames = recService.GetFileNames();

            if (fileNames != null)
                return fileNames; 

            return Problem("We can't find any saved predicts.");
        }

        // POST: api/recsystem/upload-predicts
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ActionName("upload-predicts")]
        public ActionResult UploadPredictedRatings([FromBody] List<UserCoursePredictsDTO> predicts)
        {
            var result = recService.SavePredicts(predicts);
            if (result)
                return Ok(result);
            else
                return Problem($"Something went wrong. Your data is not correct.");
        }

        // PUT: api/recsystem/apply-predict?fileName={fileName}
        [HttpPut]
        [Authorize(Roles = "admin")]
        [ActionName("apply-predict")]
        public ActionResult LoadRatingsFromFile([FromQuery] string fileName)
        {
            var result = recService.LoadRatingsFromFile(fileName);
            if (result)
                return Ok(result);
            else
                return Problem($"Something went wrong, maybe {fileName} is not exists.");
        }

        // GET: api/recsystem/get-predict/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        [ActionName("get-predict")]
        public ActionResult<UserCoursePredictsDTO?> GetUserPredict(int id)
        {
            UserCoursePredictsDTO? uc = recService.GetPredictForUser(id);

            if (uc != null)
                return uc;
            return Problem("We can't predict best course for this user.");
        }

    }
}
