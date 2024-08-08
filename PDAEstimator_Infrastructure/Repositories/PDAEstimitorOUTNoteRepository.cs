using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Data.SqlClient;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class PDAEstimitorOUTNoteRepository : IPDAEstimitorOUTNoteRepository
    {
        private readonly IConfiguration configuration;
        public PDAEstimitorOUTNoteRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(PDAEstimatorOutPutNote entity)
        {
            try
            {
                var sql = "INSERT INTO PDAEstimatorOutPutNote (PDAEstimatorOutPutID, Note, sequnce) VALUES (@PDAEstimatorOutPutID, @Note, @sequnce) SELECT CAST(SCOPE_IDENTITY() as bigint)";
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

                var sql = "Update PDAEstimatorOutPutNote set IsDeleted = 1 WHERE PDAEstimatorID = @Id";
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

        public async Task<List<PDAEstimatorOutPutNote>> GetAllAsync()
        {
            var sql = "SELECT * FROM PDAEstimatorOutPutNote where  IsDeleted! = 1 ORDER BY PDAEstimatorID DESC";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorOutPutNote>(sql);
                return new List<PDAEstimatorOutPutNote>(result.ToList());
            }
        }

        public async Task<PDAEstimatorOutPutNote> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM PDAEstimatorOutPutNote WHERE PDAEstimatorID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<PDAEstimatorOutPutNote>(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<List<PDAEstimatorOutPutNote>> GetAllNotesByPDAEstimatorOutPutIDAsync(long id)
        {
            var sql = "SELECT * FROM PDAEstimatorOutPutNote where PDAEstimatorOutPutID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorOutPutNote>(sql, new { Id = id });
                return new List<PDAEstimatorOutPutNote>(result.ToList());
            }
        }

        public async Task<int> UpdateAsync(PDAEstimatorOutPutNote entity)
        {
            try
            {
                var sql = "UPDATE PDAEstimatorOutPutNote SET CustomerId = @CustomerId,VesselName = @VesselName,PortId = @PortId,TerminalId = @TerminalId,CallTypeID = @CallTypeID,CargoId = @CargoId,CargoQty = @CargoQty, CargoQtyCBM = @CargoQtyCBM, CargoUnitofMasurement = @CargoUnitofMasurement,LoadDischargeRate = @LoadDischargeRate,CurrencyId = @CurrencyId,ROE = @ROE,DWT = @DWT,ArrivalDraft = @ArrivalDraft,GRT = @GRT,NRT = @NRT,BerthStay = @BerthStay,AnchorageStay = @AnchorageStay,LOA = @LOA,Beam = @Beam,ActivityTypeId=@ActivityTypeId ,ETA=@ETA,BerthStayDay=@BerthStayDay, InternalCompanyID = @InternalCompanyID, BerthStayShift = @BerthStayShift  WHERE PDAEstimatorID = @PDAEstimatorID";
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
