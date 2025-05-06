using AtonInternshipAssigment.Models;
using Dapper;
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

        // Создание пользователя (для администраторов)
        public async Task CreateUser(User user, string createdBy)
        {
            using var connection = new SqlConnection(_connectionString);

            user.CreatedBy = createdBy;
            user.ModifiedBy = createdBy;
            user.ModifiedOn = DateTime.UtcNow;

            await connection.ExecuteAsync(
                @"INSERT INTO Users 
            (Guid, Login, Password, Name, Gender, Birthday, Admin, 
             CreatedOn, CreatedBy, ModifiedOn, ModifiedBy) 
            VALUES 
            (@Guid, @Login, @Password, @Name, @Gender, @Birthday, @Admin, 
             @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy)",
                user);
        }

        // Получение активных пользователей (для администраторов)
        public async Task<IEnumerable<User>> GetActiveUsers()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<User>(
                "SELECT * FROM Users WHERE RevokedOn IS NULL ORDER BY CreatedOn");
        }

        public async Task UserIsAdmin(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                connection.QueryAsync(
                    "SELECT Login, Password FROM Users WHERE");
            }
            catch (Exception ex) 
            { 
            }
        }

    }
}
