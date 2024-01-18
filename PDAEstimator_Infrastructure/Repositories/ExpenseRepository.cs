using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly IConfiguration configuration;
        public ExpenseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(Expense entity)
        {
            var sql = "Insert into ExpenseMaster (ExpenseName,Status, sequnce,IsDeleted) VALUES (@ExpenseName, @Status,@sequnce,0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update ExpenseMaster set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            var sql = "SELECT * FROM ExpenseMaster where IsDeleted! = 1 order by ExpenseName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Expense>(sql);
                return new List<Expense>(result.ToList());
            }
        }


        //public async Task<List<ExpenseList>> GetAlllistAsync()
        //{
        //    var sql = "SELECT ExpenseMaster.ID as ID, ExpenseName, ExpenseMaster.CargoID as CargoID, CargoDetails.CargoName FROM ExpenseMaster left join CargoDetails on CargoDetails.ID =  ExpenseMaster.CargoID";
        //    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        var result = await connection.QueryAsync<ExpenseList>(sql);
        //        return new List<ExpenseList>(result.ToList());
        //    }
        //}
        public async Task<Expense> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM ExpenseMaster WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Expense>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Expense entity)
        {
            var sql = "UPDATE ExpenseMaster SET ExpenseName = @ExpenseName, Status = @Status,sequnce=@sequnce WHERE ID = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

       
    }
}
