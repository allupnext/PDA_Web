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
    public class CargoFamilyRepository : ICargoFamilyRepository
    {
        private readonly IConfiguration configuration;
        public CargoFamilyRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CargoFamily entity)
        {
            var sql = "Insert into CargoFamily (CargoTypeID,CargoFamilyName,CargoFamilyStatus, IsDeleted) VALUES (@CargoTypeID, @CargoFamilyName,@CargoFamilyStatus,0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            try
            {
                var sql = "Update CargoFamily set IsDeleted = 1 WHERE Id = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CargoFamily>> GetAllAsync()
        {
            var sql = "SELECT * FROM CargoFamily where  IsDeleted! = 1 order by CargoFamilyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoFamily>(sql);
                return new List<CargoFamily>(result.ToList());
            }
        }


        public async Task<List<CargoFamilyList>> GetAlllistAsync()
        {
            var sql = "SELECT CargoFamily.ID as ID, CargoFamilyName, CargoFamily.CargoTypeID as CargoTypeID, CargoFamily.CargoFamilyStatus as CargoStatus, CargoType.CargoTypeName FROM CargoFamily left join CargoType on CargoType.ID =  CargoFamily.CargoTypeID WHERE CargoFamily.IsDeleted != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoFamilyList>(sql);
                return new List<CargoFamilyList>(result.ToList());
            }
        }
        public async Task<CargoFamily> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CargoFamily WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CargoFamily>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CargoFamily entity)
        {
            var sql = "UPDATE CargoFamily SET CargoTypeID = @CargoTypeID, CargoFamilyName = @CargoFamilyName ,CargoFamilyStatus=@CargoFamilyStatus  WHERE ID = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
