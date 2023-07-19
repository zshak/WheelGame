using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Validations;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using Wheel.Extensions;
using Wheel.Models;
using Wheel.Models.Connections;
using Wheel.Models.Enums;

namespace Wheel.Repos
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly int[] levels = {100, 500, 2000, 5000, 10000};
        private readonly Connector _connector;
        private const int wheelPrizesSize = 27;
        private int prizeToGiveId = 1;
        public CustomerRepo(IOptions<Connector> connectionString)
        {
            _connector = connectionString.Value;
        }

        public async Task<int> ActivateGame(int id)
        {
            using (var connection = new NpgsqlConnection(_connector.ConnectionString))
            {
                var isUserRegisteredQuery = @"SELECT COUNT(*) FROM customers where id = @id";
                int numCustomers = await connection.QuerySingleAsync<int>(isUserRegisteredQuery, new {id});
                if (numCustomers == 0) return -1;
                var query = @"UPDATE customers SET activation_date = NOW()
                 WHERE id = @id and (activation_date is null or NOT ((activation_date >= date_trunc('day', current_timestamp)
                AND activation_date < (date_trunc('day', current_timestamp) + interval '1 day'))) )";
                var insertQuery = @"INSERT INTO daily_progress (id, date, progress,
                        amount_left_for_next_level, level)
                        VALUES (@id, NOW(), 0, 100, 0)";
                await connection.ExecuteAsync(insertQuery, new { id });
                return await connection.ExecuteAsync(query, new { id });
            }
        }


        private int GetLevel(double amount)
        {
            for(int i = 0; i < levels.Length; i++)
            {
                if(amount < levels[i]) return i;
            }
            return levels.Length;
        }
        public async Task Deposit(int id, double amount)
        {
            using (var connection = new NpgsqlConnection(_connector.ConnectionString))
            {
                var getDateQuery = @"Select date FROM daily_progress WHERE id = @id ORDER BY date DESC LIMIT 1";
                DateTime? date = await connection.QuerySingleOrDefaultAsync<DateTime?>(getDateQuery, new {id});
                bool b = date != DateTime.Today;
                if (date == null || date < DateTime.Today)
                {
                    var newDayQuery = @"UPDATE daily_progress 
                            SET date = NOW(), progress = @amount, 
                            amount_left_for_next_level = @amountLeft, level = @level, num_spins_left = num_spins_left + @numSpins
                            WHERE id = @id";
                    int level = GetLevel(amount);
                    double amountLeft = levels[level] - amount;
                    int numSpins = level;
                    await connection.ExecuteAsync(newDayQuery, new { amount, amountLeft, level, id, numSpins});
                }
                else
                {
                    var curProgressQuery = @"SELECT progress from daily_progress where id = @id";
                    int curProgress = await connection.QuerySingleAsync<int>(curProgressQuery, new { id });
                    int prevLevel = GetLevel(curProgress);
                    int level = GetLevel(amount + curProgress);
                    var sameDayQuery = @"UPDATE daily_progress
                            SET date = NOW(), progress = progress + @amount, 
                            amount_left_for_next_level = @amountLeft, level = @level, num_spins_left = num_spins_left + @numSpins
                            WHERE id = @id";
                    double amountLeft = levels[level] - amount;
                    int numSpins = level - prevLevel;
                    await connection.ExecuteAsync(sameDayQuery, new { amount, amountLeft, level, id, numSpins});
                }

                var addBetQuery = @"INSERT INTO bets (id, bet, date)
                            VALUES (@id, @amount, NOW())";
                await connection.ExecuteAsync(addBetQuery, new { id, amount });

            }
        }

        public async Task<int> GetUserByLogin(UserLoginModel user)
        {
            using(var connection = new NpgsqlConnection(_connector.ConnectionString))
            {
                var query = @"SELECT id FROM customers WHERE username = @Username and password = @Password";
                int id = await connection.QuerySingleOrDefaultAsync<int>(query, new {user.Username, Password = user.Password.HashString()});
                return id == 0? -1 : id;
            }
        }

        public async Task RegisterUser(UserRegisterModel user)
        {
            using(var connection = new NpgsqlConnection(_connector.ConnectionString))
            {
                var query = @"INSERT INTO customers (username, password, name, birth_date) VALUES (@Username, @Password, @Name, @BirthDate)";
                await connection.ExecuteAsync(query, new {user.UserName, user.Password, user.Name, user.BirthDate});
            }
        }

        public async Task<Prize?> GetPrize(int id)
        {
            using (var connection = new NpgsqlConnection(_connector.ConnectionString))
            {
                var numSpinsLeftQuey = @"SELECT num_spins_left from daily_progress where id = @id";
                int numSpinsLeft = await connection.QuerySingleOrDefaultAsync<int>(numSpinsLeftQuey, new {id});
                if (numSpinsLeft == 0) return null;
                var prizeQuery = @"SELECT prize from wheel_prizes where id = @index";
                Prize? prize = await connection.QuerySingleOrDefaultAsync<Prize>(prizeQuery, new {index = prizeToGiveId % wheelPrizesSize + 1});
                prizeToGiveId++;
                var updateLevelQuery = @"UPDATE daily_progress 
                SET num_spins_left = num_spins_left - 1
                where id = @id";
                await connection.ExecuteAsync(updateLevelQuery, new { id });
                return prize;
            }
        }
    }
}
