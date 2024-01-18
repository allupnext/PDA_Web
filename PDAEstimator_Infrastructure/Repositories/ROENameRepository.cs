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
    public class ROENameRepository : IROENameRepository
    {


        private readonly IConfiguration configuration;
        public ROENameRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(ROENames entity)
        {
            try
            {
                if (entity.IsDefault == true)
                {
                    var sql1 = "Update ROENameMaster set IsDefault = 0";
                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var result = await connection.ExecuteAsync(sql1, entity);
                    }
                }
                var sql = "Insert into ROENameMaster (ROEName,Status, IsDeleted, IsDefault) VALUES (@ROEName, @Status, 0, @IsDefault)";
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
            var sql = "Update ROENameMaster set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<ROENames>> GetAllAsync()
        {
            var sql = "SELECT * FROM ROENameMaster where  IsDeleted! = 1 order by ROEName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ROENames>(sql);
                return new List<ROENames>(result.ToList());
            }
        }



        public async Task<ROENames> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM ROENameMaster WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<ROENames>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(ROENames entity)
        {
            if (entity.IsDefault == true)
            {
                var sql1 = "Update ROENameMaster set IsDefault = 0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql1, entity);
                }
            }
            var sql = "UPDATE ROENameMaster SET ROEName=@ROEName, Status=@Status, IsDefault = @IsDefault WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }


    }
}
