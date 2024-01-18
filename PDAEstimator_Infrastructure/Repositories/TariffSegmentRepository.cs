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
    public class TariffSegmentRepository : ITariffSegment
    {
        private readonly IConfiguration configuration;
        public TariffSegmentRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(TariffSegment entity)
        {
            try
            {
                var sql = "Insert into TariffSegment (TariffSegmentName,Status, IsDeleted) VALUES (@TariffSegmentName, @Status, 0)";
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
            var sql = "Update TariffSegment set IsDeleted = 1 WHERE TariffSegmentID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<TariffSegment>> GetAllAsync()
        {
            var sql = "SELECT * FROM TariffSegment where  IsDeleted! = 1 order by TariffSegmentName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TariffSegment>(sql);
                return new List<TariffSegment>(result.ToList());
            }
        }



        public async Task<TariffSegment> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM TariffSegment WHERE TariffSegmentID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TariffSegment>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TariffSegment entity)
        {
            try
            {
                var sql = "UPDATE TariffSegment SET TariffSegmentName=@TariffSegmentName, Status=@Status WHERE TariffSegmentID = @TariffSegmentID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
