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
    public class FormulaTransactionRepository : IFormulaTransactionRepository
    {

        private readonly IConfiguration configuration;
        public FormulaTransactionRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(FormulaTransaction entity)
        {
            try {
                var sql = "Insert into FormulaTransaction (formulaID, formulaAttributeID, formulaSlabID, formulaOperatorID, formulaValue) VALUES (@formulaID, @formulaAttributeID, @formulaSlabID, @formulaOperatorID, @formulaValue) ";
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

        public async Task<int> DeleteFormulaIdAsync(int formulaID)
        {
            var sql = "DELETE FROM FormulaTransaction WHERE formulaID = @formulaID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { formulaID = formulaID });
                return result;
            }
        }

        public async Task<int> DeleteByFormulaIdAsync(int formulaID)
        {
            var sql = "DeleteFormula";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { formulaID = formulaID }, commandType: System.Data.CommandType.StoredProcedure);
                return result;
            }

        }

        public async Task<List<FormulaTransaction>> GetAllAsync()
        {
            var sql = "SELECT * FROM FormulaTransaction";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormulaTransaction>(sql);
                return new List<FormulaTransaction>(result.ToList());
            }
        }

        public async Task<List<FormularTransList>> GetAllTransAsync()
        {
            //var sql = "SELECT formulaTrasID, FormulaTransaction.formulaID as formulaID, formulaAttributeID,formula_attributes.FormulaName as formulaAttributeName, formulaSlabID, TariffSegmentName as formulaSlabName, FormulaTransaction.formulaOperatorID as formulaOperatorID, formulaOperator as formulaOperatorName, formulaValue FROM FormulaTransaction left join formula_attributes on formula_attributes.FormulaId  = FormulaTransaction.formulaAttributeID left join FormulaOperator on FormulaOperator.formulaOperatorID  = FormulaTransaction.formulaOperatorID left join TariffSegment on TariffSegment.TariffSegmentID  = FormulaTransaction.formulaSlabID";
            var sql = "GetAllFormularTransList";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormularTransList>(sql);
                return new List<FormularTransList>(result.ToList());
            }
        }

        public async Task<List<FormularTransList>> GetAllTransAsync(int formulaID)
        {
            var sql = "SELECT formulaTrasID, FormulaTransaction.formulaID as formulaID, formulaAttributeID,formula_attributes.FormulaName as formulaAttributeName, formulaSlabID, TariffSegmentName as formulaSlabName, FormulaTransaction.formulaOperatorID as formulaOperatorID, formulaOperator as formulaOperatorName, formulaValue FROM FormulaTransaction left join formula_attributes on formula_attributes.FormulaId  = FormulaTransaction.formulaAttributeID left join FormulaOperator on FormulaOperator.formulaOperatorID  = FormulaTransaction.formulaOperatorID left join TariffSegment on TariffSegment.TariffSegmentID  = FormulaTransaction.formulaSlabID where FormulaTransaction.formulaID = @formulaID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FormularTransList>(sql, new { formulaID = formulaID });
                return new List<FormularTransList>(result.ToList());
            }
        }

        public async Task<FormulaTransaction> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM FormulaTransaction WHERE formulaTrasID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FormulaTransaction>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FormulaTransaction entity)
        {
            var sql = "UPDATE FormulaTransaction SET formulaID = @formulaID,formulaAttributeID=@formulaAttributeID,formulaSlabID=@formulaSlabID,formulaOperatorID=@formulaOperatorID,formulaValue=@formulaValue WHERE formulaTrasID = @formulaTrasID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

      
        public Task<int> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
