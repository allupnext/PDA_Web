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
    public class CallTypeRepository : ICallTypeRepository
    {

        private readonly IConfiguration configuration;
        public CallTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CallType entity)
        {
            try
            {
                var sql = "Insert into CallType (CallTypeName,Status, IsDeleted) VALUES (@CallTypeName, @Status, 0)";
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

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update CallType set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<CallType>> GetAllAsync()
        {
            var sql = "SELECT * FROM CallType where  IsDeleted! = 1 order by CallTypeName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CallType>(sql);
                return new List<CallType>(result.ToList());
            }
        }

      

        public async Task<CallType> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CallType WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CallType>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CallType entity)
        {
            var sql = "UPDATE CallType SET CallTypeName=@CallTypeName, Status=@Status WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
