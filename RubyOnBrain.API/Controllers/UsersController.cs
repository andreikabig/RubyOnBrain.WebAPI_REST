using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubyOnBrain.API.Models;
using RubyOnBrain.API.Services;
using RubyOnBrain.Domain;

namespace RubyOnBrain.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Service for working with courses
        private UsersService userService;

        public UsersController(UsersService userService)
        {
            this.userService = userService;
        }

        // GET: /api/users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public List<UserDTO>? GetUsers()
        {
            return userService.GetAll();
        }

        // GET: /api/users/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult<UserDTO> GetUser(int id)
        {
            var user = userService.GetUser(id);

            if (user != null)
                return user;
            else
                return NotFound();
        }

        // POST: /api/users
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult<User> AddUser(UserDTO user)
        {
            if (user == null)
                return BadRequest();

            var result = userService.AddUser(user);
            if (result)
                return Ok(result);
            return Problem("Something went wrong. Maby this user is already exists.");
        }

        // PUT: /api/users/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult UpdateUser(int id, UserDTO user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest();
            }

            bool result = userService.ChangeUser(user);

            if (result)
                return Ok(result);

            return Problem("Something went wrong. You'r user is invalid. We can't update info.");
        }

        // DELETE: /api/users/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(int id)
        {
            bool result = userService.DeleteUser(id);

            if (result)
            {
                return Ok(result);
            }
            return Problem($"Something went wrong. We can't find user with id {id}.");
        }


        // GET: /api/users/get-ratings
        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("get-ratings")]
        public List<UserCourse> GetRatings() 
        {
            return userService.GetCurrentRatings();
        }
    }
}
