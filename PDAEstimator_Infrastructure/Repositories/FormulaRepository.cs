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
    public class FormulaRepository : IFormulaRepository
    {

        private readonly IConfiguration configuration;
        public FormulaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(FormulaMaster entity)
        {try
            {
                var sql = "Insert into FormulaMaster (formulaName,PortID, formulaStatus) VALUES (@FormulaName,@PortID, @formulaStatus)SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result =  connection.QuerySingle<int>(sql, entity);
                    return new string(result.ToString());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update FormulaMaster set IsDeleted = 1 WHERE formulaMasterID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<FormulaMaster>> GetAllAsync()
        {
            var sql = "SELECT * FROM FormulaMaster";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormulaMaster>(sql);
                return new List<FormulaMaster>(result.ToList());
            }
        }

        public async Task<FormulaMaster> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM FormulaMaster WHERE formulaMasterID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FormulaMaster>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FormulaMaster entity)
        {
            var sql = "UPDATE FormulaMaster SET formulaName = @formulaName,formulaStatus=@formulaStatus, PortID = @PortID WHERE formulaMasterID = @formulaMasterID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }


    }
}
