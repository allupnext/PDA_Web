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
    public class DesignationRepository : IDesignationRepository
    {
        private readonly IConfiguration configuration;
        public DesignationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<string> AddAsync(Designation entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Designation>> GetAllAsync()
        {
            var sql = "SELECT * FROM Designation where  IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Designation>(sql);
                return new List<Designation>(result.ToList());
            }
        }

        public async Task<Designation> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM Designation WHERE DesignationId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Designation>(sql, new { Id = id });
                return result;
            }
        }

        public Task<int> UpdateAsync(Designation entity)
        {
            throw new NotImplementedException();
        }
    }
}
