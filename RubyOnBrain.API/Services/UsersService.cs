using Microsoft.EntityFrameworkCore;
using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;

namespace RubyOnBrain.API.Services
{
    public class UsersService
    {
        private DataContext db;
        private List<User>? usersList;

        public UsersService(DataContext db)
        {
            this.db = db;
            GetUsers();
        }

        private void GetUsers()
        {
            usersList = db?.Users
                .Include(r => r.Role)
                .ToList();
        }

        public UserDTO ConvertData(User dataUser)
        {
            return new UserDTO()
            {
                Id = dataUser.Id,
                FirstName = dataUser.FirstName,
                LastName = dataUser.LastName,
                Email = dataUser.Email,
                Password = dataUser.Password,
                Role = dataUser.Role.Name,
                PhoneNumber = dataUser.PhoneNumber
            };
        }

        public UserDTO? GetUser(int id)
        {
            var user = db?.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
                return ConvertData(user);
            return null;
        }

        public List<UserDTO>? ConvertData(List<User>? dataUsers)
        { 
            List<UserDTO> users = new List<UserDTO>();

            if (dataUsers != null)
            {
                foreach (var user in dataUsers)
                {
                    users.Add(ConvertData(user));
                }

                return users; 
            }
            else
                return null;
        }

        public List<UserDTO>? GetAll() => ConvertData(usersList);

        public bool AddUser(UserDTO user)
        {
            var findUser = db?.Users.FirstOrDefault(u => u.Email == user.Email);
            var findRole = db?.Roles.FirstOrDefault(r => r.Name == user.Role);

            if (findUser == null && findRole != null && UserDataValidator(user))
            {
                db.Users.Add(new User { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Password = user.Password, PhoneNumber = user.PhoneNumber, Role = findRole });
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteUser(int id)
        {
            var user = db?.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            { 
                db?.Users.Remove(user);
                db?.SaveChanges();
                return true;
            }

            return false;
        }

        public bool ChangeUser(UserDTO user)
        {
            var _user = db?.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == user.Id);

            if (_user != null)
            { 
                var Role = db?.Roles.FirstOrDefault(r => r.Name == user.Role);

                if (Role != null)
                {
                    _user.Role = Role;
                    _user.Email = user.Email;
                    _user.Password = user.Password;
                    _user.PhoneNumber = user.PhoneNumber;
                    _user.FirstName = user.FirstName;
                    _user.LastName = user.LastName;

                    db.Users.Update(_user);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public List<UserCourse> GetCurrentRatings()
        {
            return db.UserCourses.ToList();
        }

        public bool UserDataValidator(UserDTO user) => (!String.IsNullOrEmpty(user.FirstName) && user.FirstName.Length > 1) &&
            (!String.IsNullOrEmpty(user.LastName) && user.LastName.Length > 1) &&
            (!String.IsNullOrEmpty(user.Password) && user.Password.Length > 7) && // Add Regex expressions to check the phone number
            (!String.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumber.Length >= 11);
    }
}
