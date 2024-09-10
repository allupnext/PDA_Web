using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Data.SqlClient;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class PDAEstimitorOUTRepository : IPDAEstimitorOUTRepository
    {
        private readonly IConfiguration configuration;
        public PDAEstimitorOUTRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(PDAEstimatorOutPut entity)
        {
            try
            {
                var sql = "INSERT INTO PDAEstimatorOutPut (PDAEstimatorID ,PDAEstimatorOutPutDate,CompanyName,CompanyLogo,CompanyAddress1,CompanyAddress2,CompanyTelephone,CompanyAlterTel,CompanyEmail,NameofBeneficiary,BeneficiaryAddress,AccountNo,Beneficiary_Bank_Name,Beneficiary_Bank_Address,Beneficiary_RTGS_NEFT_IFSC_Code,Beneficiary_Bank_Swift_Code,Intermediary_Bank,Intermediary_Bank_Swift_Code,BaseCurrencyCode,DefaultCurrencyCode,Taxrate, BaseCurrencyCodeID, DefaultCurrencyCodeID, Disclaimer) VALUES (@PDAEstimatorID,@PDAEstimatorOutPutDate,@CompanyName,@CompanyLogo,@CompanyAddress1,@CompanyAddress2,@CompanyTelephone,@CompanyAlterTel,@CompanyEmail,@NameofBeneficiary,@BeneficiaryAddress,@AccountNo,@Beneficiary_Bank_Name,@Beneficiary_Bank_Address,@Beneficiary_RTGS_NEFT_IFSC_Code,@Beneficiary_Bank_Swift_Code,@Intermediary_Bank,@Intermediary_Bank_Swift_Code,@BaseCurrencyCode,@DefaultCurrencyCode,@Taxrate, @BaseCurrencyCodeID, @DefaultCurrencyCodeID, @Disclaimer) SELECT CAST(SCOPE_IDENTITY() as bigint)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = connection.QuerySingle<long>(sql, entity);
                    return new string(result.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            try
            {

                var sql = "Update PDAEstimatorOutPut set IsDeleted = 1 WHERE PDAEstimatorID = @Id";
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

        public async Task<List<PDAEstimatorOutPut>> GetAllAsync()
        {
            var sql = "SELECT * FROM PDAEstimatorOutPut where  IsDeleted! = 1 ORDER BY PDAEstimatorID DESC";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorOutPut>(sql);
                return new List<PDAEstimatorOutPut>(result.ToList());
            }
        }

        public async Task<PDAEstimatorOutPut> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM PDAEstimatorOutPut WHERE PDAEstimatorID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<PDAEstimatorOutPut>(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        

        public async Task<int> UpdateAsync(PDAEstimatorOutPut entity)
        {
            try
            {
                var sql = "UPDATE PDAEstimatorOutPut SET CustomerId = @CustomerId,VesselName = @VesselName,PortId = @PortId,TerminalId = @TerminalId,CallTypeID = @CallTypeID,CargoId = @CargoId,CargoQty = @CargoQty,CargoQtyCBM = @CargoQtyCBM,  CargoUnitofMasurement = @CargoUnitofMasurement,LoadDischargeRate = @LoadDischargeRate,CurrencyId = @CurrencyId,ROE = @ROE,DWT = @DWT,ArrivalDraft = @ArrivalDraft,GRT = @GRT,NRT = @NRT,BerthStay = @BerthStay,AnchorageStay = @AnchorageStay,LOA = @LOA,Beam = @Beam,ActivityTypeId=@ActivityTypeId ,ETA=@ETA,BerthStayDay=@BerthStayDay, InternalCompanyID = @InternalCompanyID, BerthStayShift = @BerthStayShift, Disclaimer = @Disclaimer  WHERE PDAEstimatorID = @PDAEstimatorID";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return result;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
