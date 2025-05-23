﻿using AtonInternshipAssigment.Models;
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


        public async Task ChangeUserPassword(string login, string newUserPassword)
        {
            using var connection = new SqlConnection(_connectionString);

            User changeUser = new User()
            {
                Login = login,
                Password = newUserPassword,
                ModifiedOn = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Password = @Password, ModifiedBy = @Login, ModifiedOn = @ModifiedOn  WHERE Login = @Login AND RevokedOn IS NULL", changeUser);
        }


        public async Task ChangeUserLogin(string login, string newUserLogin)
        {
            using var connection = new SqlConnection(_connectionString);

            var user = new 
            {
                Login = login,
                NewLogin = newUserLogin,
                ModifiedOn = DateTime.UtcNow

            };

            await connection.ExecuteAsync(
                "UPDATE Users SET Login = @NewLogin, ModifiedBy = @Login, ModifiedOn = @ModifiedOn  WHERE Login = @Login AND RevokedOn IS NULL", user);
        }


        public async Task SoftDeleteUsersQuartz(string login, string userLogin)
        {
            using var connection = new SqlConnection(_connectionString);

            User user = new User()
            {
                RevokedOn = DateTime.UtcNow,
                RevokedBy = login,
                Login = userLogin
            };

            await connection.ExecuteAsync(
            @"UPDATE Users SET RevokedOn = @RevokedOn, RevokedBy = @RevokedBy WHERE Login = @Login", user);
        }


        public async Task HardDeleteUsersQuartz()
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(
                @"DELETE FROM Users WHERE RevokedOn <= @Threshold", new { Threshold = DateTime.UtcNow.AddDays(-3) });
        }


        public async Task HardDeleteUser(string userLogin)
        {
            using var connection = new SqlConnection(_connectionString);

            User user = new User()
            {
                Login = userLogin,
            };

            await connection.ExecuteAsync(
                @"DELETE FROM Users WHERE Login = @Login ", user);
        }


        public async Task RestoreUser(string login, string userLogin)
        {
            using var connection = new SqlConnection(_connectionString);

            User user = new User()
            {
                Login = userLogin,
                ModifiedBy = login,
                ModifiedOn = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                @"UPDATE Users SET RevokedOn = NULL, RevokedBy = NULL, ModifiedBy = @ModifiedBy, ModifiedOn = @ModifiedOn WHERE Login = @Login", user);
        }
    }
}
