using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubyOnBrain.API.Models;
using RubyOnBrain.API.Services;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private UserService userService;

        public UserController(UserService userService)
        { 
            this.userService = userService;
        }

        // GET: api/user
        [HttpGet]
        [Authorize]
        public ActionResult<UserDTO> GetCurrentUser()
        {
            var dataUser = userService.GetData(User.Identity.Name);

            if (dataUser != null)
            {
                return dataUser;
            }
            return Problem($"Something went wrong. We can't find your account.");
        }

        // PUT: /api/user
        [HttpPut]
        [Authorize]
        public ActionResult UpdateCurrentUser(UserDTO user)
        { 
            bool result = userService.UpdateUser(user);

            if (result)
                return Ok(result);

            return Problem($"Something went wrong. We can't update your account.");
        }

        // GET: /api/user/courses
        [HttpGet]
        [Authorize]
        [Route("courses")]
        public ActionResult<List<CourseDTO>> GetCourses()
        {
            var courses = userService.GetCourses(this.User.Identity.Name);

            if (courses != null)
                return courses;
            return Problem($"We can't find any course.");
        }

    }
}
