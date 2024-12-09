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
using System.Net.Mail;
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
                //var user = connection.Query<User>(sql, new { EmployCode = username, password = password }).FirstOrDefault();
                var user = connection.Query<User>(sql, new { EmployCode = username, password = password }).FirstOrDefault();
                return user;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string token, int id)
        {
            try
            {
                var sql = "update UserMaster set Token = @Token Where ID = @ID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { Token = token, ID = id });
                    return result.ToString();
                    //return new string(token.ToString(),);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<User> CheckEmailExist(string email)
        {
            var sql = "SELECT * FROM UserMaster where EmailID= @EmailID and IsDeleted = 0";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //var result = await connection.QueryAsync<User>(sql);
                //return result.FirstOrDefault();
                var User = connection.Query<User>(sql, new { EmailID = email }).FirstOrDefault();
                return User;
            }
        }

        public async Task<string> AddAsync(User entity)
        {
            try
            {
                var sql = "INSERT INTO Usermaster (FirstName, LastName, EmployCode, UserPassword, MobileNo,Salutation,Status,RoleID, EmailID, IsDeleted) VALUES (@FirstName, @LastName, @EmployCode, @UserPassword,@MobileNo,@Salutation, @Status,@RoleID ,@EmailID, 0) SELECT CAST(SCOPE_IDENTITY() as int)";
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
        public async Task<string> AddPort_User_MappingAsync(User_Port_Mapping entity)
        {
            try
            {
                var sql = "INSERT INTO  User_Port_Mapping (UserID, PortID) VALUES (@UserID, @PortID)";
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

        public async Task<int> DeletePort_User_MappingAsync(long id)
        {
            var sql = "DELETE from User_Port_Mapping WHERE UserId = @Id";
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
            try
            {
                // var sql = "SELECT UserMaster.ID as ID, FirstName,LastName,EmployCode,UserPassword,MobileNo,Salutation,EmailID,DOB,UserMaster.RoleID as RoleID,UserRole.RoleName FROM UserMaster left join UserRole on UserRole.RoleId =  UserMaster.RoleID WHERE UserMaster.IsDeleted != 1;";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<UserList>("GetAllUsers", commandType: System.Data.CommandType.StoredProcedure);
                    return new List<UserList>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<User> GetAllUsersById(long id)
        {
            try
            {
                //var sql = "SELECT * FROM CustomerMaster WHERE CustomerId = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    var dynamicParameters = new DynamicParameters();
                    var args = new Dictionary<string, object>()
                    {
                        ["UserId"] = id
                    };
                    dynamicParameters.AddDynamicParams(args);
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<User>("GetAllUsersById", new { UserId = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<User> GetByIdAsync(long id)
        {
            var sql = "SELECT UserMaster.*, UserRole.RoleName as RoleName FROM UserMaster left join UserRole on UserRole.RoleId = UserMaster.RoleID WHERE ID = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
                return result;
            }
        }
        public async Task<string> ChangePassword(string Password, long id, string macAddress)
        {
            try
            {
                var sql = "update UserMaster set UserPassword = @UserPassword,MacAddress = @macAddress Where ID = @ID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { UserPassword = Password, ID = id, MacAddress = macAddress });
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<int> AuthenticateById(int id, string Password)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    var dynamicParameters = new DynamicParameters();
                    var args = new Dictionary<string, object>()
                    {
                        ["ID"] = id,
                        ["UserPassword"] = Password
                    };
                    dynamicParameters.AddDynamicParams(args);
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<int>("AuthenticateUserById", new { ID = id, UserPassword = Password });

                    return result;
                }
            }
            catch (Exception ex) { throw ex; }


        }


        public async Task<User> GetFullUserByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT DISTINCT u.*,C.CompanyId as PrimaryCompanyId FROM UserMaster u left join Company_User_Mapping M on u.ID = M.UserID left join [CompanyMaster] C on C.CompanyId = M.CompanyID WHERE u.ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public async Task<int> UpdateAsync(User entity)
        {
            try
            {
                var sql = "UPDATE UserMaster SET FirstName = @FirstName, LastName = @LastName,EmployCode = @EmployCode, UserPassword = @UserPassword, MobileNo = @MobileNo, Salutation = @Salutation,RoleID=@RoleID, EmailID = @EmailID,Status = @Status WHERE ID = @Id";
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

        public async Task<int> AddMacAddress(string macAddress, long Id)
        {
            try
            {
                var sql = "UPDATE UserMaster SET MacAddress= @MacAddress WHERE ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { MacAddress = macAddress, ID = Id });
                    return result;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateOTP(string OTP, DateTime OTPSentDate, long Id)
        {
            try
            {
                var sql = "UPDATE UserMaster SET OTP= @OTP, OTPSentDate = @OTPSentDate  WHERE ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { OTP = OTP, OTPSentDate = OTPSentDate, Id = Id });
                    return result;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateMacAddress(string MacAddress,int ID)
        {
            try
            {
                var sql = "UPDATE UserMaster SET MacAddress= @MacAddress  WHERE ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new {MacAddress = MacAddress, ID = ID });
                    return result;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateLoginDetails(string LoginMachineName, DateTime LoginDateTime, long Id)
        {
            try
            {
                var sql = "UPDATE UserMaster SET LoginMachineName= @LoginMachineName, LoginDateTime = @LoginDateTime WHERE ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { LoginMachineName = LoginMachineName, LoginDateTime = LoginDateTime, Id = Id });
                    return result;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region User Permission
        public async Task<List<UserRolePermissionMenu>> GetAllUserRolePermissionMenuAsync()
        {
            try
            {
                var sql = "SELECT * FROM UserRolePermissionMenu";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<UserRolePermissionMenu>(sql);
                    return new List<UserRolePermissionMenu>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserRolePermissions>> GetAllUserRolePermissionsAsync()
        {
            try
            {
                var sql = "SELECT UserRolePermissions.*, UserRolePermission, IsPermission,RoleID FROM UserRolePermissions left join UserRolePermissionType on UserRolePermissionType.userRolePermissionTypeId = UserRolePermissions.userRolePermissionTypeId left join UserPemissionRole_Role_Mapping on UserPemissionRole_Role_Mapping.UserRolePermissionId = UserRolePermissions.UserRolePermissionId";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<UserRolePermissions>(sql);
                    return new List<UserRolePermissions>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserPemissionRole_Role_Mapping>> GetAllUserPemissionRole_Role_Mapping()
        {
            try
            {
                var sql = "SELECT * FROM UserPemissionRole_Role_Mapping";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<UserPemissionRole_Role_Mapping>(sql);
                    return new List<UserPemissionRole_Role_Mapping>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddUserPemissionRole_Role_MappingAsync(UserPemissionRole_Role_Mapping entity)
        {
            try
            {
                var sql = "INSERT INTO  UserPemissionRole_Role_Mapping (RoleID, UserRolePermissionId, IsPermission) VALUES (@RoleID, @UserRolePermissionId, @IsPermission)";
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

        public async Task<int> DeleteUserPemissionRole_Role_MappingAsync(int RoleID, int UserRolePermissionId)
        {
            var sql = "DELETE from UserPemissionRole_Role_Mapping WHERE RoleID = @RoleID and UserRolePermissionId = @UserRolePermissionId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { RoleID = RoleID, UserRolePermissionId = UserRolePermissionId });
                return result;
            }
        }
        #endregion

    }
}
