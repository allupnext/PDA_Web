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
    public class CurrencyRepository : ICurrencyRepository
    {


        private readonly IConfiguration configuration;
        public CurrencyRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(Currency entity)
        {
            if (entity.DefaultCurrecny == true)
            {
                var sql1 = "Update Currency set DefaultCurrecny = 0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql1, entity);
                }
            }

            if (entity.BaseCurrency == true)
            {
                var sql2 = "Update Currency set BaseCurrency = 0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql2, entity);
                }
            }

            var sql = "Insert into Currency (CurrencyName,CurrencyCode,Status, IsDeleted, BaseCurrency, DefaultCurrecny) VALUES (@CurrencyName, @CurrencyCode,@Status, 0, @BaseCurrency, @DefaultCurrecny)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update Currency set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            var sql = "SELECT * FROM Currency where  IsDeleted! = 1 order by CurrencyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Currency>(sql);
                return new List<Currency>(result.ToList());
            }
        }

        public async Task<List<Currency>> GetAllwithoutBaseCurrencyAsync()
        {
            var sql = "SELECT * FROM Currency where  IsDeleted! = 1 and BaseCurrency != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Currency>(sql);
                return new List<Currency>(result.ToList());
            }
        }

        public async Task<Currency> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Currency WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Currency>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Currency entity)
        {
            if (entity.DefaultCurrecny == true)
            {
                var sql1 = "Update Currency set DefaultCurrecny = 0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql1, entity);
                }
            }

            if (entity.BaseCurrency == true)
            {
                var sql2 = "Update Currency set BaseCurrency = 0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql2, entity);
                }
            }

            var sql = "UPDATE Currency SET CurrencyName = @CurrencyName, CurrencyCode = @CurrencyCode, Status = @Status, BaseCurrency = @BaseCurrency, DefaultCurrecny = @DefaultCurrecny WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
