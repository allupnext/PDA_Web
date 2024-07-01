using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Data.SqlClient;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class EmailNotificationConfigurationRepository : IEmailNotificationConfigurationRepository
    {

        private readonly IConfiguration configuration;
        public EmailNotificationConfigurationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(EmailNotificationConfiguration entity)
        {
            try
            {
                var sql = "Insert into EmailNotificationConfiguration (ProcessName,ProcessName, FromEmail, ToEmail) VALUES (@ProcessName, @ProcessName, @FromEmail, @ToEmail)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return new string(entity.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAsync(long EmailConfigID)
        {
            var sql = "DELETE EmailNotificationConfiguration set IsDeleted = 1 WHERE EmailConfigID = @EmailConfigID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { EmailConfigID = EmailConfigID });
                return result;
            }
        }

        public async Task<List<EmailNotificationConfiguration>> GetAllAsync()
        {
            var sql = "SELECT * FROM EmailNotificationConfiguration";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<EmailNotificationConfiguration>(sql);
                return new List<EmailNotificationConfiguration>(result.ToList());
            }
        }

      

        public async Task<EmailNotificationConfiguration> GetByIdAsync(long EmailConfigID)
        {
            var sql = "SELECT * FROM EmailNotificationConfiguration WHERE EmailConfigID = @EmailConfigID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<EmailNotificationConfiguration>(sql, new { EmailConfigID = EmailConfigID });
                return result;
            }
        }

        public async Task<int> UpdateAsync(EmailNotificationConfiguration entity)
        {
            var sql = "UPDATE EmailNotificationConfiguration SET ProcessName=@ProcessName, FromEmail=@FromEmail, ToEmail = @ToEmail WHERE EmailConfigID = @EmailConfigID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
