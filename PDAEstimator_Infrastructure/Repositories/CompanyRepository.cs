using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IConfiguration configuration;

        public CompanyRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CompanyMaster entity)
        {
            try
            {
                string telCode = entity.CountryCode + "-" + entity.Telephone;
                string alttelCode = entity.CountryCode + "-" + entity.AlterTel;
                var sql = "INSERT INTO CompanyMaster (CompanyName,CompanyLog, Address1, Address2, Country, State, City, Telephone, AlterTel, Email, Status,IsDeleted) VALUES (@CompanyName,@CompanyLog, @Address1, @Address2, @Country, @State, @City, '"+ telCode + "', '"+ alttelCode + "', @Email, @Status,0)";

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
            try { 
            var sql = "UPDATE CompanyMaster SET IsDeleted = 1 WHERE CompanyId = @Id";

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


        public async Task<List<CompanyMasterList>> GetAlllistAsync()
        {
            var sql = "SELECT CompanyId, CompanyName,CompanyLog,Address1,Address2,Telephone,AlterTel,CompanyMaster.City as City,CompanyMaster.State as State,CompanyMaster.Country as Country,Email,Status,CityName,StateName,CountryName,Telephone ,Companymaster.IsDeleted FROM CompanyMaster left join CityList on CityList.ID =  CompanyMaster.City left join Country on Country.ID =  CompanyMaster.Country left join State on State.ID =  CompanyMaster.State WHERE Companymaster.IsDeleted != 1 order by CompanyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CompanyMasterList>(sql);
                return new List<CompanyMasterList>(result.ToList());
            }
        }

        public async Task<List<CompanyMaster>> GetAllAsync()
        {
            var sql = "SELECT * FROM CompanyMaster WHERE IsDeleted != 1 order by CompanyName";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CompanyMaster>(sql);
                return new List<CompanyMaster>(result.ToList());
            }
        }

        public async Task<CompanyMaster> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CompanyMaster WHERE CompanyId = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CompanyMaster>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CompanyMaster entity)
        {
            try
            {
                string telCode = entity.CountryCode + "-" + entity.Telephone;
                string alttelCode = entity.CountryCode + "-" + entity.AlterTel;

                var sql = "UPDATE CompanyMaster SET CompanyName=@CompanyName, CompanyLog=@CompanyLog, Address1=@Address1,Address2=@Address2, Country=@Country, State=@State, City=@City, Telephone='" + telCode + "',AlterTel='" + alttelCode + "', Email=@Email, Status=@Status ,IsDeleted=@IsDeleted WHERE CompanyId = @CompanyId";

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
