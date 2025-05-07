using AtonInternshipAssigment.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AtonInternshipAssigment.Services
{
    public class CheckUserAuthorization
    {
        private readonly string _connectionString;

        public CheckUserAuthorization(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Проверка пользователя на наличие прав администратора
        public bool UserIsAdmin(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            User userAdmin = new User()
            {
                Login = login,
                Password = password
            };

            var adminExists = connection.Query(
                "SELECT * FROM Users WHERE Login = @login AND Password = @password AND Admin = 1", userAdmin);

            if (adminExists.Any())
                return true;
            else
                return false;

        }

        // Проверка на наличие пользователя
        public bool UserIsExists(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            User userIsExists = new User()
            {
                Login = login,
                Password = password
            };

            var userExists = connection.Query(
                "SELECT * FROM Users WHERE Login = @login AND Password = @password", userIsExists);

            if (userExists.Any())
                return true;
            else
                return false;
        }
    }
}
