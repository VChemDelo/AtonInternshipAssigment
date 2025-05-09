using AtonInternshipAssigment.Models;
using AtonInternshipAssigment.Repositories;
using AtonInternshipAssigment.Services;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly CheckUserAuthorization _checkUserAuthorization;
        private readonly string _connectionString;

        public AdminController(UserRepository userRepository, CheckUserAuthorization checkUserAuthorization, IConfiguration config)
        {
            _userRepository = userRepository;
            _checkUserAuthorization = checkUserAuthorization;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Добавление пользователя.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="newUserLogin">Логин нового пользователя</param>
        /// <param name="newUserPassword">Пароль нового пользователя</param>
        /// <param name="newUserName">Имя нового пользователя</param>
        /// <param name="newUserGender">Пол нового пользователя</param>
        /// <param name="birthday">Дата рождения нового пользователя</param>
        /// <param name="admin">Будут ли у пользователя права администратора</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получение активных пользователей.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveUsers(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            try
            {
                var activeUsers = connection.Query(
                    "SELECT * FROM Users WHERE RevokedOn IS NULL ORDER BY CreatedOn").ToList();

                if (activeUsers.Count != 0)
                {
                    string jsonResult = JsonSerializer.Serialize(activeUsers);
                    return Ok(jsonResult);
                }
                else 
                { 
                    return NotFound("Активных пользователей на данный момент нет");
                }
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение пользователя по логину.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsersByLogin(string login, string password, string userLogin)
        {
            using var connection = new SqlConnection(_connectionString);

            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            try
            {
                var userByLogin = connection.Query(
                    "SELECT Name, Gender, Birthday, RevokedOn FROM Users WHERE Login = @UserLogin", new {UserLogin = userLogin}).ToList();

                if(userByLogin.Count != 0)
                {
                    string jsonResult = JsonSerializer.Serialize(userByLogin);
                    return Ok(jsonResult);
                }
                else
                {
                    return NotFound("Пользователя с данным логином не существует");
                }      
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Получение информации о пользователе.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserByLoginAndPassword(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return BadRequest("Неверный логин или пароль!");

            try
            {
                var userByLoginAndPassword = connection.Query(
                    "SELECT Login, Password, Name, Gender, Birthday FROM Users WHERE Login = @Login", new { Login = login }).ToList();

                    string jsonResult = JsonSerializer.Serialize(userByLoginAndPassword);
                    return Ok(jsonResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Получение пользователей старше определенного возраста.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="ageThreshold">Значение старше какого возраста необходимо получить пользователей</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserOlderThan(string login, string password, int ageThreshold)
        {
            using var connection = new SqlConnection(_connectionString);

            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            if (ageThreshold < 0 || ageThreshold > 100)
                return BadRequest("Возраст не может быть больше 100 лет и меньше 0 лет.");

            try
            {
                DateTime cutoffDate = DateTime.UtcNow.AddYears(-ageThreshold);

                var userByLogin = connection.Query(
                    "SELECT * FROM Users WHERE Birthday < @CutoffDate", new { CutoffDate = cutoffDate}).ToList();

                if (userByLogin.Count != 0)
                {
                    string jsonResult = JsonSerializer.Serialize(userByLogin);
                    return Ok(jsonResult);
                }
                else
                {
                    return NotFound("Пользователя с данным логином не существует");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Мягкое удаление пользователя.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> SoftDeleteUsers(string login, string password, string userLogin)
        {
            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            try
            {
                await _userRepository.SoftDeleteUsersQuartz(login, userLogin);

                return Ok("Пользователь отмечен на удаление. Он будет удален через 3 дня");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Принудительное удаление пользователя.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> HardDeleteUser(string login, string password, string userLogin)
        {
            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            try
            {
                await _userRepository.HardDeleteUser(userLogin);

                return Ok("Пользователь удален");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Восстановление пользователя.
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RestoreUser(string login, string password, string userLogin)
        {
            var userIsAdmin = _checkUserAuthorization.UserIsAdmin(login, password);

            if (!userIsAdmin)
                return Unauthorized("Неверный логин или пароль!");

            try
            {
                await _userRepository.RestoreUser(login, userLogin);

                return Ok("Пользователь убран из списка на мягкое удаление");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
