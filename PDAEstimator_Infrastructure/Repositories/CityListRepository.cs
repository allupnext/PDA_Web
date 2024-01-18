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
    public class CityListRepository : ICityListRepository
    {
        private readonly IConfiguration configuration;
        public CityListRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CityList entity)
        {
            try
            {
                var sql = "Insert into CityList (CityName,StateId, IsDeleted) VALUES (@CityName,@StateId, 0)";
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
            var sql = "Update CityList set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }



        public async Task<List<CityList>> GetAllAsync()
        {
            var sql = "SELECT * FROM CityList where  IsDeleted! = 1 order by CityName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CityList>(sql);
                return new List<CityList>(result.ToList());
            }
        }

        public async Task<List<CityList>> GetCitylistByCountry(int Country, int State)
        {
            var sql = "select L.ID as ID,L.CityName as CityName from CityList L inner join State S on L.StateId=S.ID where CountryId=" + Country;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CityList>(sql);
                return new List<CityList>(result.ToList());
            }
        }

        public async Task<CityList> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CityList WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CityList>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CityList entity)
        {
            var sql = "UPDATE CityList SET CityName = @CityName, StateId = @StateId WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
