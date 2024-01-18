using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;
        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
            

        public async Task<User> Authenticate(string username, string password)
        {
            var sql = "SELECT * FROM UserMaster where EmployCode=@EmployCode and UserPassword=@password";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //var result = await connection.QueryAsync<User>(sql);
                //return result.FirstOrDefault();
                var user = connection.Query<User>(sql, new { EmployCode = username, password = password }).FirstOrDefault();
                return user;
            }
        }

        public async Task<string> AddAsync(User entity)
        {
            try
            {
                var sql = "INSERT INTO Usermaster (FirstName, LastName, EmployCode, UserPassword, MobileNo,Salutation,Status,RoleID, IsDeleted) VALUES (@FirstName, @LastName, @EmployCode, @UserPassword,@MobileNo,@Salutation, @Status,@RoleID, 0) SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = connection.QuerySingle<int>(sql, entity);
                    return new string(result.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddCustomer_User_MappingAsync(Company_User_Mapping entity)
        {
            try
            {
                var sql = "INSERT INTO  Company_User_Mapping (CompanyID, UserID, IsPrimary) VALUES (@CompanyID, @UserID, @IsPrimary)";
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

        public async Task<int> DeleteCustomer_User_MappingAsync(long id)
        {
            var sql = "DELETE from Company_User_Mapping WHERE UserId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update Usermaster set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            var sql = "SELECT * FROM UserMaster where  IsDeleted! = 1 order by FirstName,LastName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(sql);
                return new List<User>(result.ToList());
            }
        }

        public async Task<List<UserList>> GetAlllistAsync()
        {
           // var sql = "SELECT UserMaster.ID as ID, FirstName,LastName,EmployCode,UserPassword,MobileNo,Salutation,EmailID,DOB,UserMaster.RoleID as RoleID,UserRole.RoleName FROM UserMaster left join UserRole on UserRole.RoleId =  UserMaster.RoleID WHERE UserMaster.IsDeleted != 1;";
           
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UserList>("GetAllUsers", commandType: System.Data.CommandType.StoredProcedure);
                return new List<UserList>(result.ToList());
            }
        }


        public async Task<User> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM UserMaster WHERE ID = @Id";
            
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
        }
        public async Task<User> GetFullUserByIdAsync(long id)
        {
            var sql = "SELECT DISTINCT u.*,C.CompanyId as PrimaryCompanyId FROM UserMaster u left join Company_User_Mapping M on u.ID = M.UserID left join [CompanyMaster] C on C.CompanyId = M.CompanyID WHERE u.ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(User entity)
        {
            try
            {
                var sql = "UPDATE UserMaster SET FirstName = @FirstName, LastName = @LastName,EmployCode = @EmployCode, UserPassword = @UserPassword, MobileNo = @MobileNo, Salutation = @Salutation,RoleID=@RoleID,Status = @Status WHERE ID = @Id";
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
