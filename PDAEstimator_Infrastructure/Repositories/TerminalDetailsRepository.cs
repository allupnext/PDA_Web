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
    public class TerminalDetailsRepository : ITerminalDetailsRepository
    {
        private readonly IConfiguration configuration;
        public TerminalDetailsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(TerminalDetails entity)
        {
            var sql = "Insert into TerminalDetails (TerminalCode, TerminalName, Status, PortID , IsDeleted) VALUES (@TerminalCode ,@TerminalName, @Status, @PortID, 0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update TerminalDetails set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<TerminalDetails>> GetAllAsync()
        {
            var sql = "SELECT TerminalDetails.*, PortName FROM TerminalDetails Left join PortDetails On PortDetails.ID = TerminalDetails.PortID where  TerminalDetails.IsDeleted != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TerminalDetails>(sql);
                return new List<TerminalDetails>(result.ToList());
            }
        }

        public async Task<TerminalDetails> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM TerminalDetails WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TerminalDetails>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<TerminalDetails>> GetByPortIdAsync(long id)
        {
            //var sql = "SELECT TerminalDetails.*, PortName FROM TerminalDetails Left join PortDetails On PortDetails.ID = TerminalDetails.PortID where  TerminalDetails.IsDeleted != 1";

            var sql = "SELECT TerminalDetails.*, PortName FROM TerminalDetails Left join PortDetails On PortDetails.ID = TerminalDetails.PortID where  TerminalDetails.IsDeleted != 1 and PortID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TerminalDetails>(sql, new { Id = id });
                return new List<TerminalDetails>(result.ToList());
            }
        }

        public async Task<int> UpdateAsync(TerminalDetails entity)
        {
            var sql = "UPDATE TerminalDetails SET TerminalCode = @TerminalCode, TerminalName = @TerminalName, Status = @Status, PortID = @PortID WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
