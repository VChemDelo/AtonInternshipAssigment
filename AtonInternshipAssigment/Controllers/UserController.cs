using AtonInternshipAssigment.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string login, string password, string newUserLogin, string newUserPassword, string newUserName, int newUserGender, DateTime Birthday, bool admin)
        {

        }
    }
}
