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
    public class DisclaimersRepository : IDisclaimersRepository
    {
        private readonly IConfiguration _configuration;

        public DisclaimersRepository(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public async Task<string> AddAsync(Disclaimers entity)
        {
            try
            {
                if(entity.IsActive == true)
                {
                    var sqlUpdate = "Update Disclaimers set IsActive = 0";
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        await connection.ExecuteAsync(sqlUpdate);
                        //return result;
                    }
                }
                var sql = "Insert into Disclaimers (Disclaimer,IsActive,IsDeleted) VALUES (@Disclaimer, @IsActive,0)";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
            var sql = "Update Disclaimers set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Disclaimers>> GetAllAsync()
        {
            var sql = "SELECT * FROM Disclaimers where IsDeleted != 1";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Disclaimers>(sql);
                return new List<Disclaimers>(result.ToList());
            }
        }

        public async Task<Disclaimers> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Disclaimers WHERE ID = @Id and IsDeleted != 1";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Disclaimers>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Disclaimers entity)
        {
            if (entity.IsActive == true)
            {
                var sqlUpdate = "Update Disclaimers set IsActive = 0";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    await connection.ExecuteAsync(sqlUpdate);
                    //return result;
                }
            }
            var sql = "UPDATE Disclaimers SET Disclaimer=@Disclaimer, IsActive=@IsActive, IsDeleted=@IsDeleted WHERE ID = @Id";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
