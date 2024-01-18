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
    public class PortActivityTypeRepository : IPortActivityTypeRepository
    {
        private readonly IConfiguration configuration;
        public PortActivityTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        

        public async Task<string> AddAsync(PortActivityType entity)
        {
            var sql = "Insert into PortActivityType (ActivityType,Status) VALUES (@ActivityType,1)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }
        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update PortActivityType set Status = 0 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }


        public async Task<List<PortActivityType>> GetAllAsync()
        {
            var sql = "select ID,ActivityType From PortActivityType where Status=1 order by ActivityType";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PortActivityType>(sql);
                return new List<PortActivityType>(result.ToList());
            }
        }

        public async Task<PortActivityType> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM PortActivityType WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<PortActivityType>(sql, new { Id = id });
                return result;
            }
        }



        public async Task<int> UpdateAsync(PortActivityType entity)
        {
            var sql = "UPDATE PortActivityType SET ActivityType = @ActivityType WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
