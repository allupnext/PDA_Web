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
    public class FormatProviderRepository : IFormulaAttributesRepository
    {
        private readonly IConfiguration configuration;
        public FormatProviderRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(FormulaAttributes entity)
        {
            var sql = "Insert into formula_attributes (FormulaName,Status, IsDeleted)VALUES (@FormulaName, @Status, 0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update formula_attributes set IsDeleted = 1 WHERE FormulaId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<FormulaAttributes>> GetAllAsync()
        {
            var sql = "SELECT * FROM formula_attributes where  IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormulaAttributes>(sql);
                return new List<FormulaAttributes>(result.ToList());
            }
        }

        public async Task<FormulaAttributes> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM formula_attributes WHERE FormulaId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FormulaAttributes>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FormulaAttributes entity)
        {
            var sql = "UPDATE formula_attributes SET FormulaName = @FormulaName,Status=@Status WHERE FormulaId = @FormulaId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

    }
}
