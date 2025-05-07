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
        public async Task CreateUser(string login, string newUserLogin, string newUserPassword, string newUserName, int newUserGender, DateTime birthday, bool admin)
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

        // Изменение имени пользователя
        public async Task ChangeUserName(string login, string newUserName)
        {
            using var connection = new SqlConnection(_connectionString);

            User changeUser = new User()
            {
                Login = login,
                Name = newUserName
            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Name = @Name WHERE Login = @Login AND RevokedOn IS NULL", changeUser);
        }

        // Изменение пола пользователя
        public async Task ChangeUserGender(string login, int newUserGender)
        {
            using var connection = new SqlConnection(_connectionString);

            User changeUser = new User()
            {
                Login = login,
                Gender = newUserGender,
                ModifiedOn = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Gender = @Gender, ModifiedBy = @Login, ModifiedOn = @ModifiedOn WHERE Login = @Login AND RevokedOn IS NULL", changeUser);
        }

        // Изменение дня рождения пользователя
        public async Task ChangeUserBirthday(string login, DateTime newBirthday)
        {
            using var connection = new SqlConnection(_connectionString);

            User changeUser = new User()
            {
                Login = login,
                Birthday = newBirthday,
                ModifiedOn = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Birthday = @Birthday, ModifiedBy = @Login, ModifiedOn = @ModifiedOn  WHERE Login = @Login AND RevokedOn IS NULL", changeUser);
        }
    }
}
