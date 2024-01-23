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
    public class NotesRepository : INotesRepository
    {
        private readonly IConfiguration _configuration;

        public NotesRepository(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public async Task<string> AddAsync(Notes entity)
        {
            try
            {
                var sql = "Insert into Notes (Note,sequnce,IsActive,IsDeleted) VALUES (@Note, @sequnce, @IsActive,0)";
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
            var sql = "Update Notes set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Notes>> GetAllAsync()
        {
            var sql = "SELECT * FROM Notes where IsDeleted != 1";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Notes>(sql);
                return new List<Notes>(result.ToList());
            }
        }

        public async Task<Notes> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Notes WHERE ID = @Id and where IsDeleted != 1";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Notes>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Notes entity)
        {
            var sql = "UPDATE Notes SET Note=@Note, IsActive=@IsActive,sequnce=@sequnce,IsDeleted=@IsDeleted WHERE ID = @Id";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
