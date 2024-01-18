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
    public class RolesRepository : IRoleRepository
    {
        private readonly IConfiguration configuration;
        public RolesRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(Roles entity)
        {
            var sql = "Insert into UserRole (RoleName,Status, IsDeleted)VALUES (@RoleName, @Status, 0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update UserRole set IsDeleted = 1 WHERE RoleId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<Roles>> GetAllAsync()
        {
            var sql = "SELECT * FROM UserRole where  IsDeleted! = 1 order by RoleName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Roles>(sql);
                return new List<Roles>(result.ToList());
            }
        }

        public async Task<Roles> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM UserRole WHERE RoleId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Roles>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Roles entity)
        {
            var sql = "UPDATE UserRole SET RoleName = @RoleName,Status=@Status WHERE RoleId = @RoleId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
