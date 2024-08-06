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
                var sql = "Insert into EmailNotificationConfiguration (CompanyId,ProcessName, FromEmail, ToEmail) VALUES (@CompanyId, @ProcessName, @FromEmail, @ToEmail)";
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
            var sql = "SELECT EmailNotificationConfiguration.*, CompanyName as CompneyName FROM EmailNotificationConfiguration LEFT Join CompanyMaster On CompanyMaster.CompanyId = EmailNotificationConfiguration.CompanyId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<EmailNotificationConfiguration>(sql);
                return new List<EmailNotificationConfiguration>(result.ToList());
            }
        }

        public async Task<EmailNotificationConfiguration> GetByIdAsync(long EmailConfigID)
        {
            var sql = "SELECT EmailNotificationConfiguration.*, CompanyName as CompneyName FROM EmailNotificationConfiguration LEFT Join CompanyMaster On CompanyMaster.CompanyId = EmailNotificationConfiguration.CompanyId WHERE EmailConfigID = @EmailConfigID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<EmailNotificationConfiguration>(sql, new { EmailConfigID = EmailConfigID });
                return result;
            }
        }

        public async Task<EmailNotificationConfiguration> GetByProcessNameAsync(string ProcessName)
        {
            var sql = "SELECT * FROM EmailNotificationConfiguration WHERE ProcessName = @ProcessName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<EmailNotificationConfiguration>(sql, new { ProcessName = ProcessName });
                return result;
            }
        }

        public async Task<EmailNotificationConfiguration> GetByCompanyandProcessNameAsync(int CompanyId, string ProcessName)
        {
            var sql = "SELECT * FROM EmailNotificationConfiguration WHERE CompanyId = @CompanyId and ProcessName = @ProcessName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<EmailNotificationConfiguration>(sql, new { CompanyId = CompanyId, ProcessName = ProcessName });
                return result;
            }
        }

        public async Task<int> UpdateAsync(EmailNotificationConfiguration entity)
        {
            var sql = "UPDATE EmailNotificationConfiguration SET CompanyId=@CompanyId, ProcessName=@ProcessName, FromEmail=@FromEmail, ToEmail = @ToEmail WHERE EmailConfigID = @EmailConfigID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
