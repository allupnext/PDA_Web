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
    public class BankMasterRepository : IBankMasterRepository
    {
        private readonly IConfiguration configuration;
        public BankMasterRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        

        public async Task<string> AddAsync(BankMaster entity)
        {
            try
            {
                var sql = "Insert into BankMaster (CompanyId,NameofBeneficiary,BeneficiaryAddress,AccountNo,Beneficiary_Bank_Name,Beneficiary_Bank_Address,Beneficiary_RTGS_NEFT_IFSC_Code,Beneficiary_Bank_Swift_Code,Intermediary_Bank,Intermediary_Bank_Swift_Code,Bank_Code,IsDefault,Status) VALUES (@CompanyId,@NameofBeneficiary,@BeneficiaryAddress,@AccountNo,@Beneficiary_Bank_Name,@Beneficiary_Bank_Address,@Beneficiary_RTGS_NEFT_IFSC_Code,@Beneficiary_Bank_Swift_Code,@Intermediary_Bank,@Intermediary_Bank_Swift_Code,@Bank_Code,@IsDefault,1)";
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
            var sql = "Update BankMaster set Status=  0  WHERE BankId= @BankId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { BankId = id });
                return result;
            }
        }


        public async Task<List<BankMaster>> GetAllAsync()
        {
            var sql = "select * From BankMaster where Status=1 order by NameofBeneficiary";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<BankMaster>(sql);
                return new List<BankMaster>(result.ToList());
            }
        }
        public async Task<List<BankMaster>> GetAllBankDetailsAsync()
        {
            var sql = "select * From BankMaster where Status=1 order by Bank_Code";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<BankMaster>(sql);
                return new List<BankMaster>(result.ToList());
            }
        }

        public async Task<BankMaster> GetByIdAsync(long id)
        {
            var sql = "select * From BankMaster WHERE BankId= @BankId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<BankMaster>(sql, new { BankId = id });
                return result;
            }
        }

        public async Task<BankMaster> GetByCompanyIdAsync(int id)
        {
            var sql = "select top 1 * From BankMaster WHERE CompanyId= @CompanyId and IsDefault = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<BankMaster>(sql, new { CompanyId = id });
                return result;
            }
        }



        public async Task<int> UpdateAsync(BankMaster entity)
        {
            try
            {
                var sql = "Update BankMaster set CompanyId=@CompanyId,NameofBeneficiary=@NameofBeneficiary,BeneficiaryAddress=@BeneficiaryAddress,AccountNo=@AccountNo,Beneficiary_Bank_Name=@Beneficiary_Bank_Name,Beneficiary_Bank_Address=@Beneficiary_Bank_Address,Beneficiary_RTGS_NEFT_IFSC_Code=@Beneficiary_RTGS_NEFT_IFSC_Code,Beneficiary_Bank_Swift_Code=@Beneficiary_Bank_Swift_Code,Intermediary_Bank=@Intermediary_Bank,Intermediary_Bank_Swift_Code=@Intermediary_Bank_Swift_Code,Bank_Code=@Bank_Code,IsDefault=@IsDefault WHERE BankId= @BankId";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return result;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
