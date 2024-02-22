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
    public class CountryRepository : ICountryRepository
    {
        private readonly IConfiguration configuration;
        public CountryRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(Country entity)
        {
            var sql = "Insert into Country (CountryName, CountryCode,IsDeleted) VALUES (@CountryName,@CountryCode,0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update Country set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Country>> GetAllAsync()
        {
            var sql = "SELECT * FROM Country where  IsDeleted! = 1 order by CountryName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Country>(sql);
                return new List<Country>(result.ToList());
            }
        }

        public async Task<Country> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Country WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Country>(sql, new { Id = id });
                return result;
            }
        }
        public async Task<Country> GetCountryCodeByCountryIdAsync(long id)
        {
            var sql = "select CountryCode from Country Where ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Country>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Country entity)
        {
            var sql = "UPDATE Country SET CountryName = @CountryName,CountryCode=@CountryCode WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
