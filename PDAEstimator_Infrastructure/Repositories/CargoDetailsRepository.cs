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
    public class CargoDetailsRepository : ICargoDetailsRepository
    {
        private readonly IConfiguration configuration;
        public CargoDetailsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CargoDetails entity)
        {
            try
            {
                var sql = "Insert into CargoDetails (CargoTypeID,CargoName, CargoStatus, CargoFamilyID, CargoFile, IsDeleted) VALUES (@CargoTypeID, @CargoName, @CargoStatus, @CargoFamilyID, @CargoFile,0)";
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
            var sql = "Update CargoDetails set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<CargoDetails>> GetAllAsync()
        {
            var sql = "SELECT * FROM CargoDetails where  IsDeleted! = 1 order by CargoName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoDetails>(sql);
                return new List<CargoDetails>(result.ToList());
            }
        }

        public async Task<List<CargoDetails>> GetAllPortIdAsync(int id)
        {
            var sql = "SELECT * FROM CargoDetails WHERE ProdId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoDetails>(sql, new { ProdId = id });
                return new List<CargoDetails>(result.ToList());
            }
        }

        public async Task<List<CargoDetailsList>> GetAlllistAsync()
        {
            var sql = "SELECT CargoFile, CargoDetails.ID as ID, CargoName, CargoDetails.CargoTypeID as CargoTypeID, CargoDetails.CargoFamilyID as CargoFamilyID, CargoDetails.CargoStatus as CargoStatus, CargoTypeName,CargoFamilyName FROM CargoDetails left join CargoType on CargoType.ID =  CargoDetails.CargoTypeID left join CargoFamily on CargoFamily.ID =  CargoDetails.CargoFamilyID WHERE CargoDetails.IsDeleted != 1 order by CargoName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoDetailsList>(sql);
                return new List<CargoDetailsList>(result.ToList());
            }
        }


        public async Task<CargoDetails> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CargoDetails WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CargoDetails>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CargoDetails entity)
        {
            var sql = "UPDATE CargoDetails SET CargoTypeID=@CargoTypeID, CargoName=@CargoName, CargoStatus = @CargoStatus, CargoFamilyID = @CargoFamilyID, CargoFile = @CargoFile WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
