﻿using Dapper;
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
    public class CustomerUser : ICustomerUserMaster
    {
        private readonly IConfiguration configuration;
        public CustomerUser(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CustomerUserMaster entity)
        {
            try
            {
                string mobwithCode = entity.CountryCode + "-" + entity.Mobile;
                string teleWithCode = entity.AlterCountryCode + "-" + entity.Telephone;

                var sql = "INSERT INTO CustomerUserMaster(Salutation, FirstName, LastName, Designation, Address1, Address2,Company, City, State, Country, Email, Mobile, Password, CustomerId, IsDeleted,Telephone,AlternativeEmail)VALUES (@Salutation, @FirstName, @LastName, @Designation, @Address1, @Address2,@Company, @City, @State, @Country, @Email, '" + mobwithCode + "' , @Password, @CustomerId, 0,'" + teleWithCode + "' ,@AlternativeEmail) SELECT CAST(SCOPE_IDENTITY() as int)";
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
        public async Task<List<CustomerUserMaster>> GetAllAsync()
        {
            var sql = "SELECT * FROM CustomerUserMaster where IsDeleted != 1 order by FirstName,LastName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CustomerUserMaster>(sql);
                return new List<CustomerUserMaster>(result.ToList());
            }
        }
        public async Task<List<CustomerUserMaster>> GetByCustomerIdAsync(long id)
        {
            //var sql = "SELECT TerminalDetails.*, PortName FROM TerminalDetails Left join PortDetails On PortDetails.ID = TerminalDetails.PortID where  TerminalDetails.IsDeleted != 1";
            try
            {
                var sql = "select CustomerUserMAster.* , CustomerMaster.Company from CustomerUserMAster left join CustomerMaster on CustomerMaster.CustomerId = CustomerUserMaster.CustomerId where CustomerUserMAster.IsDeleted != 1 and CustomerMaster.CustomerId = @CustomerId";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<CustomerUserMaster>(sql, new { CustomerId = id });
                    return new List<CustomerUserMaster>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerUserMaster> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM CustomerUserMaster WHERE ID = @ID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
/*                    var dynamicParameters = new DynamicParameters();
                    var args = new Dictionary<string, object>()
                    {
                        ["CustomerId"] = id
                    };
                    dynamicParameters.AddDynamicParams(args);*/
                    connection.Open();
                     var result = await connection.QuerySingleOrDefaultAsync<CustomerUserMaster>(sql ,new { ID = id});
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<int> UpdateAsync(CustomerUserMaster entity)
        {
            string mobwithCode = entity.CountryCode + "-" + entity.Mobile;
            string teleWithCode = entity.AlterCountryCode + "-" + entity.Telephone;
            try
            {
                //var sql = "UPDATE CustomerMaster SET Beneficiary_Bank_Name=@Beneficiary_Bank_Name,Salutation = @Salutation, FirstName = @FirstName,LastName = @LastName, Designation = @Designation, Address1 = @Address1, Address2 = @Address2,Company=@Company,City = @City,State = @State,Country = @Country,Email = @Email,Mobile = '" + mobwithCode + "',Telephone = '" + teleWithCode + "',Password = @Password,Status = @Status,IsEmailNotification = @IsEmailNotification WHERE CustomerId = @CustomerId";
                var sql = "UPDATE CustomerUserMaster SET Salutation = @Salutation, FirstName = @FirstName,LastName = @LastName, Designation = @Designation, Address1 = @Address1, Address2 = @Address2,Company=@Company,City = @City,State = @State,Country = @Country,Email = @Email,Mobile = '" + mobwithCode + "',Telephone = '" + teleWithCode + "',AlternativeEmail = @AlternativeEmail,Password = @Password WHERE ID = @ID";
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
        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update CustomerUserMaster set IsDeleted = 1 WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

    }
}