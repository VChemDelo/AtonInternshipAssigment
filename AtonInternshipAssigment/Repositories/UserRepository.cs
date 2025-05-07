using AtonInternshipAssigment.Models;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace AtonInternshipAssigment.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Добавление нового пользователя
        public async Task CreateUser(string login, string password, string newUserLogin, string newUserPassword, string newUserName, int newUserGender, DateTime birthday, bool admin)
        {
            using var connection = new SqlConnection(_connectionString);

            User createUser = new User()
            {
                Guid = Guid.NewGuid(),
                Login = newUserLogin,
                Password = newUserPassword,
                Name = newUserName,
                Gender = newUserGender,
                Birthday = birthday,
                Admin = admin,
                CreatedBy = login,
                CreatedOn = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                @"INSERT INTO Users (Guid, Login, Password, Name, Gender, Birthday, Admin, CreatedBy, CreatedOn) 
                VALUES 
                (@Guid, @Login, @Password, @Name, @Gender, @Birthday, @Admin, @CreatedBy, @CreatedOn)", createUser);
        }

        public async Task ChangeUserName(string login, string password, string newUserName)
        {
            using var connection = new SqlConnection(_connectionString);

            User changeUser = new User()
            {
                Login = login,
                Password = password,
                Name = newUserName,
            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Name = @Name WHERE Login = @Login", changeUser);
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

            var userExists = connection.QueryAsync(
                "SELECT * FROM Users WHERE Login = @login AND Password = @password AND Admin = 1", userAdmin);

            if(userExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
               
        }

    }
}
