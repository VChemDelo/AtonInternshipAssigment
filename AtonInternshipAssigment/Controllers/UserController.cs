using AtonInternshipAssigment.Models;
using AtonInternshipAssigment.Repositories;
using AtonInternshipAssigment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly CheckUserAuthorization _checkUserAuthorization;

        public UserController(UserRepository userRepository, CheckUserAuthorization checkUserAuthorization) 
        { 
            _userRepository = userRepository;
            _checkUserAuthorization = checkUserAuthorization;
        }

        // Изменение имени
        [HttpPatch]
        public async Task<IActionResult> ChangeUserName(string login,string password, string newUserName)
        {
            var regex = new Regex("^[a-zA-Zа-яА-Я]+$", RegexOptions.CultureInvariant);
            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return Unauthorized("Неверный логин или пароль!");

            var validateNewName = regex.IsMatch(newUserName);

            if (!validateNewName)
                return BadRequest("Имя пользователя указанно некорректно. Запрещены все символы кроме латинских и русских букв");
            try
            {
                await _userRepository.ChangeUserName(login, newUserName);

                return Ok("Имя пользователя успешно изменено.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение пола
        [HttpPatch]
        public async Task<IActionResult> ChangeUserGender(string login, string password, int newUserGender)
        {
            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return Unauthorized("Неверный логин или пароль!");

            if (newUserGender > 2 || newUserGender < 0)
                return BadRequest("Новый пол пользователя указан некорректно. Доступны следующие варианты полов: 0 - женщина, 1 - мужчина, 2 - неизвестно.");

            try
            {
                await _userRepository.ChangeUserGender(login, newUserGender);

                return Ok("Пол пользователя успешно изменено.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Изменение даты рождения 
        [HttpPatch]
        public async Task<IActionResult> ChangeUserBirthday(string login, string password, DateTime newBirthday)
        {
            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return Unauthorized("Неверный логин или пароль!");

            if (newBirthday >= DateTime.UtcNow)
                return BadRequest("Дата не может быть в будущем.");

            try
            {
                await _userRepository.ChangeUserBirthday(login, newBirthday);

                return Ok("Дата рождения пользователя успешно изменена.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение пароля
        [HttpPatch] 
        public async Task<IActionResult> ChangeUserPassword(string login, string password, string newUserPassword)
        {
            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return Unauthorized("Неверный логин или пароль!");

            var regex = new Regex("^[a-zA-Z0-9]+$");

            var validateNewPassword = regex.IsMatch(newUserPassword);

            if (!validateNewPassword)
                return BadRequest("Новый пароль указанно некорректно. Запрещены все символы кроме латинских букв и цифр.");

            try
            {
                await _userRepository.ChangeUserPassword(login, newUserPassword);

                return Ok("Пароль пользователя успешно изменен.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение логина
        [HttpPatch]
        public async Task<IActionResult> ChangeUserLogin(string login, string password, string newUserLogin)
        {
            var userIsExists = _checkUserAuthorization.UserIsExists(login, password);

            if (!userIsExists)
                return Unauthorized("Неверный логин или пароль!");

            var regex = new Regex("^[a-zA-Z0-9]+$");

            var validateNewLogin = regex.IsMatch(newUserLogin);

            if (!validateNewLogin)
                return BadRequest("Новый пароль указанно некорректно. Запрещены все символы кроме латинских букв и цифр.");

            try
            {
                await _userRepository.ChangeUserLogin(login, newUserLogin);

                return Ok("Логин пользователя успешно изменен.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
