using AtonInternshipAssigment.Repositories;
using AtonInternshipAssigment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly CheckUserAuthorization _checkUserAuthorization;

        public AdminController(UserRepository userRepository, CheckUserAuthorization checkUserAuthorization)
        {
            _userRepository = userRepository;
            _checkUserAuthorization = checkUserAuthorization;
        }

        // Добавление нового пользователя
        [HttpPost]
        public async Task<IActionResult> CreateUser(string login, string password, string newUserLogin, string newUserPassword, string newUserName, int newUserGender, DateTime birthday, bool admin)
        {
            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            var userIsExists = _checkUserAuthorization.UserIsExists(newUserLogin, newUserPassword);

            if (userIsExists)
                return BadRequest("Пользователь с таким логином уже существует!");

            try
            {
                await _userRepository.CreateUser(login, newUserLogin, newUserPassword, newUserName, newUserGender, birthday, admin);

                return Ok("Пользователь создан!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
