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
    public class ChargeCodeRepository : IChargeCodeRepository
    {
        private readonly IConfiguration configuration;
        public ChargeCodeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(ChargeCode entity)
        {
            try
            {
                var sql = "Insert into ChargeCodeMaster (ChargeCodeName, ExpenseCategoryID, Status, CreationDate, IPAddress, IsDeleted,Sequence) VALUES ( @ChargeCodeName, @ExpenseCategoryID, @Status, getdate(), 1, 0,@Sequence)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return new string(entity.ToString());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update ChargeCodeMaster set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<ChargeCode>> GetAllAsync()
        {
            var sql = "SELECT * FROM ChargeCodeMaster where  IsDeleted! = 1 order by ChargeCodeName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ChargeCode>(sql);
                return new List<ChargeCode>(result.ToList());
            }
        }


        public async Task<List<ChargeCodeList>> GetAlllistAsync()
        {
            var sql = "SELECT ChargeCodeMaster.ID as ID, ChargeCodeName, ChargeCodeMaster.ExpenseCategoryID as ExpenseCategoryID,ExpenseMaster.ExpenseName,Sequence, ChargeCodeMaster.Status FROM ChargeCodeMaster left join ExpenseMaster on ExpenseMaster.ID =  ChargeCodeMaster.ExpenseCategoryID WHERE ChargeCodeMaster.IsDeleted != 1";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ChargeCodeList>(sql);
                return new List<ChargeCodeList>(result.ToList());
            }
        }
        public async Task<ChargeCode> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM ChargeCodeMaster WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<ChargeCode>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(ChargeCode entity)
        {
            try
            {
                var sql = "UPDATE ChargeCodeMaster SET ChargeCodeName = @ChargeCodeName, ExpenseCategoryID = @ExpenseCategoryID ,Status=@Status,Sequence = @Sequence WHERE ID = @Id";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return result;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
