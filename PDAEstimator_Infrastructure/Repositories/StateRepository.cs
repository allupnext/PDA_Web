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
    public class StateRepository : IStateRepository
    {
        private readonly IConfiguration configuration;
        public StateRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(State entity)
        {
            var sql = "Insert into State (StateName,CountryId, IsDeleted)VALUES (@StateName,@CountryId, 0)"
;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update State set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<State>> GetAllAsync()
        {
            var sql = "SELECT * FROM State where  IsDeleted! = 1 order by StateName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<State>(sql);
                return new List<State>(result.ToList());
            }
        }

        public async Task<State> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM State WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<State>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(State entity)
        {
            var sql = "UPDATE State SET StateName = @StateName,CountryId=@CountryId WHERE Id = @Id ";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
