using AtonInternshipAssigment.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly UserRepository _userRepository;

        public AdminController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Добавление нового пользователя
        [HttpPost]
        public async Task<IActionResult> CreateUser(string login, string password, string newUserLogin, string newUserPassword, string newUserName, int newUserGender, DateTime birthday, bool admin)
        {
            var userIsADmin = _userRepository.UserIsAdmin(login, password);

            if (!userIsADmin)
            {
                return Unauthorized("Неверный логин или пароль!");
            }

            try
            {
                await _userRepository.CreateUser(login, password, newUserLogin, newUserPassword, newUserName, newUserGender, birthday, admin);

                return Ok("Пользователь создан!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
