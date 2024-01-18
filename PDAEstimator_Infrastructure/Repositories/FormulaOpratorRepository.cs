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
    public class FormulaOpratorRepository : IFormulaOpratorRepository
    {
        private readonly IConfiguration configuration;
        public FormulaOpratorRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<string> AddAsync(FormulaOprator entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FormulaOprator>> GetAllAsync()
        {
            var sql = "SELECT * FROM FormulaOperator where  IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormulaOprator>(sql);
                return new List<FormulaOprator>(result.ToList());
            }
        }

        public async Task<FormulaOprator> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM FormulaOperator WHERE formulaOperatorID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FormulaOprator>(sql, new { Id = id });
                return result;
            }
        }

        public Task<int> UpdateAsync(FormulaOprator entity)
        {
            throw new NotImplementedException();
        }
    }
}
