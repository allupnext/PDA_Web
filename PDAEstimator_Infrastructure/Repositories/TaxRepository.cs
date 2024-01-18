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
    public class TaxRepository : ITaxRepository
    {
        private readonly IConfiguration configuration;
        public TaxRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(Tax entity)
        {
            try
            {
                var sql = "insert into Taxmaster (TaxName,TaxRate,CombineTax,CombineTaxID,FromDate,ToDate) values(@TaxName,@TaxRate,@CombineTax,@CombineTaxID,@FromDate,@ToDate)";
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
            
            var sql = "Update Taxmaster set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Tax>> GetAllAsync()
        {
            var sql = "select T.ID,T.TaxName,T.TaxRate,isnull(TM.TaxName,'') as CombineTaxName,T.FromDate,T.ToDate From taxmaster T left join taxmaster TM on T.CombineTaxID=TM.ID where  T.IsDeleted! = 1 order by TaxName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Tax>(sql);
                return new List<Tax>(result.ToList());
            }
        }



        public async Task<Tax> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Taxmaster WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Tax>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Tax entity)
        {
            var sql = "UPDATE Taxmaster SET TaxName=@TaxName, TaxRate=@TaxRate,CombineTax=@CombineTax,CombineTaxID=@CombineTaxID,FromDate=@FromDate,ToDate=@ToDate WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
