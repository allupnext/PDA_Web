using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Data.SqlClient;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class PDAEstimatorOutPutTariffRepository : IPDAEstimatorOutPutTariffRepository
    {
        private readonly IConfiguration configuration;
        public PDAEstimatorOutPutTariffRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(PDAEstimatorOutPutTariff entity)
        {
            try
            {
                var sql = "INSERT INTO PDAEstimatorOutPutTariff (PDAEstimatorOutPutID ,ExpenseCategoryID,ChargeCodeName,Rate,UNITS,Amount,Remark, Taxrate, TaxID, CurrencyID) VALUES (@PDAEstimatorOutPutID , @ExpenseCategoryID, @ChargeCodeName, @Rate, @UNITS, @Amount, @Remark, @Taxrate, @TaxID, @CurrencyID) SELECT CAST(SCOPE_IDENTITY() as bigint)";
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

                var sql = "Update PDAEstimatorOutPutTariff set IsDeleted = 1 WHERE PDAEstimatorID = @Id";
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

        public async Task<List<PDAEstimatorOutPutTariff>> GetAllAsync()
        {
            var sql = "SELECT * FROM PDAEstimatorOutPutTariff where  IsDeleted! = 1 ORDER BY PDAEstimatorID DESC";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorOutPutTariff>(sql);
                return new List<PDAEstimatorOutPutTariff>(result.ToList());
            }
        }

        public async Task<PDAEstimatorOutPutTariff> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM PDAEstimatorOutPutTariff WHERE PDAEstimatorID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<PDAEstimatorOutPutTariff>(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<List<PDAEstimatorOutPutTariff>> GetAllByPDAEstimatorIDAsync(long id)
        {
            var sql = "SELECT * FROM PDAEstimatorOutPutTariff where PDAEstimatorOutPutID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorOutPutTariff>(sql, new { Id = id });
                return new List<PDAEstimatorOutPutTariff>(result.ToList());
            }
        }

        public async Task<int> UpdateAsync(PDAEstimatorOutPutTariff entity)
        {
            try
            {
                var sql = "UPDATE PDAEstimatorOutPutTariff SET CustomerId = @CustomerId,VesselName = @VesselName,PortId = @PortId,TerminalId = @TerminalId,CallTypeID = @CallTypeID,CargoId = @CargoId,CargoQty = @CargoQty,CargoQtyCBM = @CargoQtyCBM, CargoUnitofMasurement = @CargoUnitofMasurement,LoadDischargeRate = @LoadDischargeRate,CurrencyId = @CurrencyId,ROE = @ROE,DWT = @DWT,ArrivalDraft = @ArrivalDraft,GRT = @GRT,NRT = @NRT,BerthStay = @BerthStay,AnchorageStay = @AnchorageStay,LOA = @LOA,Beam = @Beam,ActivityTypeId=@ActivityTypeId ,ETA=@ETA,BerthStayDay=@BerthStayDay, InternalCompanyID = @InternalCompanyID, BerthStayShift = @BerthStayShift  WHERE PDAEstimatorID = @PDAEstimatorID";
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
