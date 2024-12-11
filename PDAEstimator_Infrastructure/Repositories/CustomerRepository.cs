using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration configuration;
        public CustomerRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<CustomerUserMaster> Authenticate(string email, string password)
        {
            var sql = "SELECT * FROM CustomerUserMaster where Email=@Email and Password=@Password and IsDeleted != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //var result = await connection.QueryAsync<User>(sql);
                //return result.FirstOrDefault();
                var customer = connection.Query<CustomerUserMaster>(sql, new { Email = email, password = password }).FirstOrDefault();
                return customer;
            }
        }

        public async Task<int> UpdateMacAddress(string MacAddress, int ID)
        {
            try
            {
                var sql = "UPDATE CustomerUserMaster SET MacAddress= @MacAddress  WHERE ID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { MacAddress = MacAddress, ID = ID });
                    return result;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerUserMaster> CheckEmailExist(string email)
        {
            var sql = "SELECT * FROM CustomerUserMaster where Email=@Email and IsDeleted != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //var result = await connection.QueryAsync<User>(sql);
                //return result.FirstOrDefault();
                var customer = connection.Query<CustomerUserMaster>(sql, new { Email = email}).FirstOrDefault();
                return customer;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string token, int id)
        {
            try
            {
                //var sql = "update CustomerMaster set Token = @Token Where CustomerId = @CustomerId";
                var sql = "update CustomerUserMaster set Token = @Token Where ID = @ID";
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

        public async Task<string> ChangePassword(string Password, long id, string macAddress, string loginMachineName)
        {
            try
            {
                //var sql = "update CustomerMaster set Password = @Password Where CustomerId = @CustomerId";
                var sql = "update CustomerUserMaster set Password = @Password,MacAddress = @macAddress, LoginMachineName = @loginMachineName Where ID = @ID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { Password = Password, ID = id, MacAddress = macAddress, LoginMachineName = loginMachineName });
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<string> ChangePasswordByCurrent(string Password, long id)
        {
            try
            {
                //var sql = "update CustomerMaster set Password = @Password Where CustomerId = @CustomerId";
                var sql = "update CustomerUserMaster set Password = @Password Where ID = @ID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { Password = Password, ID = id });
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> AuthenticateById(int id, string Password )
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    var dynamicParameters = new DynamicParameters();
                    var args = new Dictionary<string, object>()
                    {
                        ["ID"] = id,
                        ["Password"] = Password
                    };
                    dynamicParameters.AddDynamicParams(args);
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<int>("AuthenticateById", new { ID = id, Password = Password });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }


        }

        public async Task<string> AddAsync(Customer entity)
        {
            try
            {
               string mobwithCode = entity.CountryCode +"-"+ entity.Mobile; 

                //var sql = "INSERT INTO CustomerMaster(BankID,Salutation, FirstName, LastName, Designation, Address1, Address2,Company, City, State, Country, Email, Mobile, Password, Status, IsDeleted,Telephone,AlternativeEmail,IsEmailNotification)VALUES (@BankID,@Salutation, @FirstName, @LastName, @Designation, @Address1, @Address2,@Company, @City, @State, @Country, @Email, '" + mobwithCode + "' , @Password, @Status,0,'" + teleWithCode + "' ,@AlternativeEmail,@IsEmailNotification) SELECT CAST(SCOPE_IDENTITY() as int)";
                var sql = "INSERT INTO CustomerMaster(BankID,Company, Status, IsDeleted,IsEmailNotification, Email, Mobile, CreatedBy, CreatedDate)VALUES (@BankID,@Company, @Status,0,@IsEmailNotification,@Email, '" + mobwithCode + "' ,@CreatedBy, @CreatedDate) SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = connection.QuerySingle<int>(sql, entity);
                    return new string(result.ToString());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
          
        }

        public async Task<string> AddCustomer_Company_MappingAsync(Company_Customer_Mapping entity)
        {
            try
            {
                var sql = "INSERT INTO  Company_Customer_Mapping (CompanyID, CustomerID, IsPrimary) VALUES (@CompanyID, @CustomerID, @IsPrimary)";
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

        public async Task<int> DeleteCustomer_Company_MappingAsync(long id)
        {
            var sql = "DELETE from Company_Customer_Mapping WHERE CustomerId = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "DeleteCustomerById";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            var sql = "SELECT * FROM CustomerMaster where IsDeleted != 1 order by Company";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Customer>(sql);
                return new List<Customer>(result.ToList());
            }
        }

        public async Task<Customer> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM CustomerMaster WHERE CustomerId = @Id and IsDeleted != 1";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    var dynamicParameters = new DynamicParameters();
                    var args = new Dictionary<string, object>()
                    {
                        ["CustomerId"] = id
                    };
                    dynamicParameters.AddDynamicParams(args);
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<Customer>("GetCustomerById", new { CustomerId = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }
    
        public async Task<List<CustomerList>> GetAlllistAsync()
        {
            //var sql = "SELECT CustomerId, Salutation,FirstName,LastName,Designation,Address1,Address2,Company,CustomerMaster.City as City,CustomerMaster.State as State,CustomerMaster.Country as Country,Email,Mobile,Password,CityName,StateName,CountryName,AlternativeEmail,Telephone,IsEmailNotification,BankMaster.Bank_Code FROM CustomerMaster left join CityList on CityList.ID =  CustomerMaster.City left join Country on Country.ID =  CustomerMaster.Country left join State on State.ID =  CustomerMaster.State Left join BankMaster ON BankMaster.BankId = CustomerMaster.BankID WHERE CustomerMaster.IsDeleted != 1";
            var sql = "SELECT CustomerId,Company,IsEmailNotification,BankMaster.Bank_Code, CreatedBy, ModifyBy, ModifyDate  CreatedDate FROM CustomerMaster Left join BankMaster ON BankMaster.BankId = CustomerMaster.BankID WHERE CustomerMaster.IsDeleted != 1";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CustomerList>("GetAllCustomers", commandType: System.Data.CommandType.StoredProcedure);
                return new List<CustomerList>(result.ToList());
            }
        }

        public async Task<List<CustomerList>> GetAlllistCustomerAsync(int customerId)
        {
            //var sql = "SELECT CustomerId, Salutation,FirstName,LastName,Designation,Address1,Address2,Company,CustomerMaster.City as City,CustomerMaster.State as State,CustomerMaster.Country as Country,Email,Mobile,Password,Status,CityName,StateName,CountryName FROM CustomerMaster left join CityList on CityList.ID =  CustomerMaster.City left join Country on Country.ID =  CustomerMaster.Country left join State on State.ID =  CustomerMaster.State WHERE CustomerMaster.IsDeleted != 1";
            var sql = "SELECT CustomerId,Company,Status FROM CustomerMaster  WHERE CustomerMaster.IsDeleted != 1 ";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CustomerList>(sql, new { customerId = customerId });
                return new List<CustomerList>(result.ToList());
            }
        }

        public async Task<int> UpdateAsync(Customer entity)
        {
           string mobwithCode = entity.CountryCode + "-" + entity.Mobile;
            try
            {
                //var sql = "UPDATE CustomerMaster SET Beneficiary_Bank_Name=@Beneficiary_Bank_Name,Salutation = @Salutation, FirstName = @FirstName,LastName = @LastName, Designation = @Designation, Address1 = @Address1, Address2 = @Address2,Company=@Company,City = @City,State = @State,Country = @Country,Email = @Email,Mobile = '" + mobwithCode + "',Telephone = '" + teleWithCode + "',Password = @Password,Status = @Status,IsEmailNotification = @IsEmailNotification WHERE CustomerId = @CustomerId";
                var sql = "UPDATE CustomerMaster SET BankID = @BankID,Company=@Company,Status = @Status,IsEmailNotification = @IsEmailNotification,Email = @Email,Mobile = '" + mobwithCode + "', CreatedBy = @CreatedBy, CreatedDate = @CreatedDate WHERE CustomerId = @CustomerId";
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


    }
}
