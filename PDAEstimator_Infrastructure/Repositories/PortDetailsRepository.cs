using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class PortDetailsRepository : IPortDetailsRepository
    {
        private readonly IConfiguration configuration;
        public PortDetailsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(PortDetails entity)
        {
            var sql = "Insert into PortDetails (PortCode,PortName,City, State, Country, Status, PortFile,PortFileTanKer, IsDeleted) VALUES (@PortCode,@PortName,@City,@State,@Country, @Status, @PortFile,@PortFileTanker, 0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update PortDetails set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<PortDetails>> GetAllAsync()
        {
            var sql = "SELECT * FROM PortDetails where  IsDeleted! = 1  ORDER BY PortName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PortDetails>(sql);
                return new List<PortDetails>(result.ToList());
            }
        }

        public async Task<PortDetails> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM PortDetails WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<PortDetails>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(PortDetails entity)
        {
            var sql = "UPDATE PortDetails SET PortCode= @PortCode,PortName=@PortName, City=@City, State=@State, Country =@Country, Status= @Status, PortFile =@PortFile, PortFileTanker =@PortFileTanker WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
