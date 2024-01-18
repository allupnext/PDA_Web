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
    public class CargoTypeRepository : ICargoTypeRepository
    {
        private readonly IConfiguration configuration;
        public CargoTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CargoType entity)
        {
            var sql = "Insert into CargoType (CargoTypeName,CargoTypeStatus, IsDeleted) VALUES (@CargoTypeName, @CargoTypeStatus,0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update CargoType set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<CargoType>> GetAllAsync()
        {
            var sql = "SELECT * FROM CargoType where  IsDeleted! = 1 order by CargoTypeName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoType>(sql);
                return new List<CargoType>(result.ToList());
            }
        }

        public async Task<CargoType> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CargoType WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CargoType>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CargoType entity)
        {
            var sql = "UPDATE CargoType SET CargoTypeName = @CargoTypeName, CargoTypeStatus = @CargoTypeStatus WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
