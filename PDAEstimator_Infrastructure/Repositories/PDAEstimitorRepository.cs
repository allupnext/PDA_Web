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
    public class PDAEstimitorRepository : IPDAEstimitorRepository
    {
        private readonly IConfiguration configuration;
        public PDAEstimitorRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(PDAEstimator entity)
        {
            try
            {
                var sql = "INSERT INTO PDAEstimator (CreatedBy,CreationDate, CustomerID, VesselName, PortID, TerminalID,BerthId, CallTypeID, CargoID, CargoQty,CargoQtyCBM, CargoUnitofMasurement, LoadDischargeRate, CurrencyID, ROE, DWT, ArrivalDraft, GRT, NRT,BerthStay, AnchorageStay, LOA, Beam, IsDeleted,ActivityTypeId,ETA,BerthStayDay, InternalCompanyID, BerthStayShift,VesselBallast,IsReducedGRT, BerthStayDayCoastal, BerthStayShiftCoastal, BerthStayHoursCoastal, RGRT, IsCustomerCreated) VALUES (@CreatedBy,GetDate(), @CustomerID, @VesselName, @PortID, @TerminalID,@BerthId, @CallTypeID, @CargoID, @CargoQty,@CargoQtyCBM, @CargoUnitofMasurement, @LoadDischargeRate, @CurrencyID, @ROE, @DWT, @ArrivalDraft, @GRT,@NRT, @BerthStay, @AnchorageStay, @LOA, @Beam, 0 , @ActivityTypeId ,@ETA,@BerthStayDay, @InternalCompanyID, @BerthStayShift , @VesselBallast ,@IsReducedGRT, @BerthStayDayCoastal, @BerthStayShiftCoastal, @BerthStayHoursCoastal, @RGRT, @IsCustomerCreated) SELECT CAST(SCOPE_IDENTITY() as bigint)";
        
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

                var sql = "Update PDAEstimator set IsDeleted = 1 WHERE PDAEstimatorID = @Id";
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

        public async Task<List<PDAEstimator>> GetAllAsync()
        {
            var sql = "SELECT * FROM PDAEstimator where  IsDeleted! = 1 ORDER BY PDAEstimatorID DESC";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimator>(sql);
                return new List<PDAEstimator>(result.ToList());
            }
        }

        public async Task<Company_Customer_Mapping> GetbyCustIdasync(int id)
        {
            var sql = "SELECT TOP (1000) [CompanyID],[CustomerID],[IsPrimary] FROM [Company_Customer_Mapping] where [CustomerID] = @CustomerID  and [IsPrimary] = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Company_Customer_Mapping>(sql, new { CustomerID = id });
                return  result;

            }
        }

        public async Task<List<PDATariffRateList>> GetAllPDA_Tariff(int portId, DateTime ETA)
        {
            var sql = "GetAllPDA_Tariff";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDATariffRateList>(sql, new { PortId = portId, ETA = ETA.Date });
                return new List<PDATariffRateList>(result.ToList());
            }
        }




        public async Task<List<CargoDetails>> GetCargoByTerminalAndPortAsync(int terminalId, int portId)
        {
            try
            {
                if (terminalId > 0)
                {
                    var sql = @"
                SELECT DISTINCT CargoDetails.*
                FROM CargoDetails
                left JOIN CargoHandled ON CargoDetails.ID = CargoHandled.CargoID and CargoHandled.IsDeleted = 0
                WHERE CargoHandled.TerminalID = @TerminalId AND CargoHandled.PortID = @PortId and CargoDetails.Isdeleted = 0";

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var result = await connection.QueryAsync<CargoDetails>(sql, new { TerminalId = terminalId, PortId = portId });
                        return result.ToList();
                    }
                }
                else
                {
                    var sql = @"
                SELECT DISTINCT CargoDetails.*
                FROM CargoDetails
                left JOIN CargoHandled ON CargoDetails.ID = CargoHandled.CargoID and CargoHandled.IsDeleted = 0
                WHERE CargoHandled.PortID = @PortId and CargoDetails.Isdeleted = 0";

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var result = await connection.QueryAsync<CargoDetails>(sql, new { PortId = portId });
                        return result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TerminalDetails>> GetTerminalByCargoIdAndPortAsync(int CargoID, int PortID)
        {
            try
            {
                if (CargoID > 0)
                {
                    var sql = @"
                SELECT Distinct TerminalDetails.* FROM TerminalDetails 
                Left Join CargoHandled ON TerminalDetails.ID = CargoHandled.TerminalID and CargoHandled.IsDeleted = 0 where CargoHandled.CargoID = @CargoID and TerminalDetails.PortID = @PortID and TerminalDetails.Isdeleted = 0";

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var result = await connection.QueryAsync<TerminalDetails>(sql, new { CargoID = CargoID, PortId = PortID });
                        return result.ToList();
                    }
                }
                else
                {
                    var sql = @"
                 SELECT Distinct TerminalDetails.* FROM TerminalDetails 
                Left Join CargoHandled ON TerminalDetails.ID = CargoHandled.TerminalID and CargoHandled.IsDeleted = 0 where TerminalDetails.PortID = @PortID and TerminalDetails.Isdeleted = 0";

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var result = await connection.QueryAsync<TerminalDetails>(sql, new { PortId = PortID });
                        return result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<PDAEstimatorList>> GetAlllistAsync()
        {
            try
            {
                //var sql = "SELECT PDAEstimator.PDAEstimatorID as PDAEstimatorID, PDAEstimator.CustomerID as CustomerID,CustomerMaster.Company as CustomerCompanyName,PDAEstimator.PortID as PortID, PDAEstimator.TerminalID as TerminalID,PDAEstimator.CallTypeID as CallTypeID,PDAEstimator.CargoID as CargoID,PDAEstimator.ActivityTypeId as ActivityTypeId,PDAEstimator.CurrencyID as CurrencyID,CurrencyCode as CurrencyName,BerthStayDay,ETA,CargoQty,CargoUnitofMasurement,LoadDischargeRate,PDAEstimator.CurrencyID as CurrencyID,ROE,DWT,ArrivalDraft,GRT,NRT,BerthStay,VesselName,AnchorageStay,LOA,Beam,FirstName,ActivityType,PortName,TerminalName,CallTypeName,CargoName,InternalCompanyID, CompanyMaster.CompanyName as InternalCompanyName, BerthStayShift, VesselBallast, BerthStayDayCoastal, BerthStayShiftCoastal, BerthStayHoursCoastal  FROM PDAEstimator  left join CustomerMaster on CustomerMaster.CustomerId =  PDAEstimator.CustomerID  left join PortDetails on PortDetails.ID =  PDAEstimator.PortID  left join TerminalDetails on TerminalDetails.ID =  PDAEstimator.TerminalID left join CallType on CallType.ID =  PDAEstimator.CallTypeID left join Currency on Currency.ID =  PDAEstimator.CurrencyID left join PortActivityType on PortActivityType.ID =  PDAEstimator.ActivityTypeId left join CargoDetails on CargoDetails.ID =  PDAEstimator.CargoID left join CompanyMaster on CompanyMaster.CompanyId =  PDAEstimator.InternalCompanyID WHERE PDAEstimator.IsDeleted != 1 ORDER BY PDAEstimatorID DESC";
                var sql = "SELECT PDAEstimator.ModifyUserID as ModifyUserID, ModifyUser.EmployCode as ModifyUser, PDAEstimator.CreatedBy as UserId,CreatedByUser.EmployCode as CreatedBy,  PDAEstimator.PDAEstimatorID as PDAEstimatorID, PDAEstimator.CustomerID as CustomerID, CustomerMaster.Company as CustomerCompanyName, PDAEstimator.PortID as PortID, PDAEstimator.TerminalID as TerminalID, PDAEstimator.BerthId as BerthId, PDAEstimator.CallTypeID as CallTypeID, PDAEstimator.CargoID as CargoID, PDAEstimator.ActivityTypeId as ActivityTypeId, PDAEstimator.CurrencyID as CurrencyID, CurrencyCode as CurrencyName, BerthStayDay,ETA,CargoQty,CargoQtyCBM, CargoUnitofMasurement,LoadDischargeRate, PDAEstimator.CurrencyID as CurrencyID, ROE,PDAEstimator.DWT,ArrivalDraft,GRT,RGRT,NRT,BerthStay, VesselName,AnchorageStay,LOA,Beam,CustomerMaster.FirstName,ActivityType,PortName,TerminalName,BerthName, CallTypeName,CargoName,InternalCompanyID, CompanyMaster.CompanyName as InternalCompanyName, BerthStayShift, VesselBallast,IsReducedGRT, BerthStayDayCoastal, BerthStayShiftCoastal, BerthStayHoursCoastal , PDAEstimator.ModifyDate,PDAEstimator.CreationDate  FROM PDAEstimator  left join UserMaster AS ModifyUser on PDAEstimator.ModifyUserID = ModifyUser.ID left join UserMaster AS CreatedByUser on PDAEstimator.CreatedBy = CreatedByUser.ID left join CustomerMaster on CustomerMaster.CustomerId =  PDAEstimator.CustomerID   left join PortDetails on PortDetails.ID =  PDAEstimator.PortID left join TerminalDetails on TerminalDetails.ID =  PDAEstimator.TerminalID left join BerthDetails on BerthDetails.Id = PDAEstimator.BerthID left join CallType on CallType.ID =  PDAEstimator.CallTypeID left join Currency on Currency.ID =  PDAEstimator.CurrencyID left join PortActivityType on PortActivityType.ID =  PDAEstimator.ActivityTypeId left join CargoDetails on CargoDetails.ID =  PDAEstimator.CargoID left join CompanyMaster on CompanyMaster.CompanyId =  PDAEstimator.InternalCompanyID WHERE PDAEstimator.IsDeleted != 1  ORDER BY PDAEstimatorID DESC ";
                
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<PDAEstimatorList>(sql);
                    return new List<PDAEstimatorList>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<PDAEstimatorList>> GetAlllistByCustIdAsync(int CustomerID)
        {
            try
            {
                //var sql = "SELECT PDAEstimator.PDAEstimatorID as PDAEstimatorID, PDAEstimator.CustomerID as CustomerID,CustomerMaster.Company as CustomerCompanyName,PDAEstimator.PortID as PortID, PDAEstimator.TerminalID as TerminalID,PDAEstimator.CallTypeID as CallTypeID,PDAEstimator.CargoID as CargoID,PDAEstimator.ActivityTypeId as ActivityTypeId,PDAEstimator.CurrencyID as CurrencyID,CurrencyCode as CurrencyName,BerthStayDay,ETA,CargoQty,CargoUnitofMasurement,LoadDischargeRate,PDAEstimator.CurrencyID as CurrencyID,ROE,DWT,ArrivalDraft,GRT,NRT,BerthStay,VesselName,AnchorageStay,LOA,Beam,FirstName,ActivityType,PortName,TerminalName,CallTypeName,CargoName,InternalCompanyID, CompanyMaster.CompanyName as InternalCompanyName, BerthStayShift, VesselBallast, BerthStayDayCoastal, BerthStayShiftCoastal, BerthStayHoursCoastal  FROM PDAEstimator  left join CustomerMaster on CustomerMaster.CustomerId =  PDAEstimator.CustomerID  left join PortDetails on PortDetails.ID =  PDAEstimator.PortID  left join TerminalDetails on TerminalDetails.ID =  PDAEstimator.TerminalID left join CallType on CallType.ID =  PDAEstimator.CallTypeID left join Currency on Currency.ID =  PDAEstimator.CurrencyID left join PortActivityType on PortActivityType.ID =  PDAEstimator.ActivityTypeId left join CargoDetails on CargoDetails.ID =  PDAEstimator.CargoID left join CompanyMaster on CompanyMaster.CompanyId =  PDAEstimator.InternalCompanyID WHERE PDAEstimator.IsDeleted != 1 ORDER BY PDAEstimatorID DESC";
                var sql = "SELECT PDAEstimator.ModifyUserID as ModifyUserID, ModifyUser.EmployCode as ModifyUser, PDAEstimator.CreatedBy as UserId,CreatedByUser.EmployCode as CreatedBy,  PDAEstimator.PDAEstimatorID as PDAEstimatorID, PDAEstimator.CustomerID as CustomerID, CustomerMaster.Company as CustomerCompanyName, PDAEstimator.PortID as PortID, PDAEstimator.TerminalID as TerminalID, PDAEstimator.BerthId as BerthId, PDAEstimator.CallTypeID as CallTypeID, PDAEstimator.CargoID as CargoID, PDAEstimator.ActivityTypeId as ActivityTypeId, PDAEstimator.CurrencyID as CurrencyID, CurrencyCode as CurrencyName, BerthStayDay,ETA,CargoQty,CargoQtyCBM, CargoUnitofMasurement,LoadDischargeRate, PDAEstimator.CurrencyID as CurrencyID, ROE,PDAEstimator.DWT,ArrivalDraft,GRT,RGRT,NRT,BerthStay, VesselName,AnchorageStay,LOA,Beam,CustomerMaster.FirstName,ActivityType,PortName,TerminalName,BerthName, CallTypeName,CargoName,InternalCompanyID, CompanyMaster.CompanyName as InternalCompanyName, BerthStayShift, VesselBallast,IsReducedGRT, BerthStayDayCoastal, BerthStayShiftCoastal, BerthStayHoursCoastal , PDAEstimator.ModifyDate,PDAEstimator.CreationDate  FROM PDAEstimator  left join UserMaster AS ModifyUser on PDAEstimator.ModifyUserID = ModifyUser.ID left join UserMaster AS CreatedByUser on PDAEstimator.CreatedBy = CreatedByUser.ID left join CustomerMaster on CustomerMaster.CustomerId =  PDAEstimator.CustomerID   left join PortDetails on PortDetails.ID =  PDAEstimator.PortID left join TerminalDetails on TerminalDetails.ID =  PDAEstimator.TerminalID left join BerthDetails on BerthDetails.Id = PDAEstimator.BerthID left join CallType on CallType.ID =  PDAEstimator.CallTypeID left join Currency on Currency.ID =  PDAEstimator.CurrencyID left join PortActivityType on PortActivityType.ID =  PDAEstimator.ActivityTypeId left join CargoDetails on CargoDetails.ID =  PDAEstimator.CargoID left join CompanyMaster on CompanyMaster.CompanyId =  PDAEstimator.InternalCompanyID WHERE PDAEstimator.CustomerID = @CustomerID and PDAEstimator.IsDeleted != 1 and PDAEstimator.IsCustomerCreated = 1  ORDER BY PDAEstimatorID DESC";

                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<PDAEstimatorList>(sql, new { CustomerID = CustomerID });
                    return new List<PDAEstimatorList>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<PDAEstimatorList>> GetPDAEstiomatorListOfLast30Days()
        {
            var sql = "GetPDAEstiomatorListOfLast30Days";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PDAEstimatorList>(sql);
                return new List<PDAEstimatorList>(result.ToList());
            }
        }


        public async Task<List<Notes>> GetNotes()
        {
            var sql = "select Note from notes where IsActive=1 order by sequnce";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Notes>(sql);
                return new List<Notes>(result.ToList());
            }
        }

        public async Task<PDAEstimator> GetByIdAsync(long id)
        {
            try
            {
                var sql = "SELECT * FROM PDAEstimator WHERE PDAEstimatorID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<PDAEstimator>(sql, new { Id = id });
                    return result;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        //public async Task<List<PDAEstimitorList>> GetAlllistAsync()
        //{
        //    var sql = "SELECT PDAEstimitorId, Salutation,FirstName,LastName,Designation,Address1,Address2,Company,PDAEstimitorMaster.City as City,PDAEstimitorMaster.State as State,PDAEstimitorMaster.Country as Country,Email,Mobile,Password,Status,CityName,StateName,CountryName FROM PDAEstimitorMaster left join CityList on CityList.ID =  PDAEstimitorMaster.City left join Country on Country.ID =  PDAEstimitorMaster.Country left join State on State.ID =  PDAEstimitorMaster.State";

        //    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        var result = await connection.QueryAsync<PDAEstimitorList>(sql);
        //        return new List<PDAEstimitorList>(result.ToList());
        //    }
        //}
        public async Task<int> UpdateAsync(PDAEstimator entity)
        {
            try
            {
                var sql = "UPDATE PDAEstimator SET ModifyUserID = @ModifyUserID,ModifyDate = GetDate(),CustomerId = @CustomerId,VesselName = @VesselName,PortId = @PortId,TerminalId = @TerminalId,BerthId = @BerthId,CallTypeID = @CallTypeID,CargoId = @CargoId,CargoQty = @CargoQty,CargoQtyCBM = @CargoQtyCBM, CargoUnitofMasurement = @CargoUnitofMasurement,LoadDischargeRate = @LoadDischargeRate,CurrencyId = @CurrencyId,ROE = @ROE,DWT = @DWT,ArrivalDraft = @ArrivalDraft,GRT = @GRT,RGRT = @RGRT,NRT = @NRT,BerthStay = @BerthStay,AnchorageStay = @AnchorageStay,LOA = @LOA,Beam = @Beam,ActivityTypeId=@ActivityTypeId ,ETA=@ETA,BerthStayDay=@BerthStayDay, InternalCompanyID = @InternalCompanyID, BerthStayShift = @BerthStayShift, VesselBallast = @VesselBallast,IsReducedGRT = @IsReducedGRT BerthStayDayCoastal = @BerthStayDayCoastal, BerthStayShiftCoastal = @BerthStayShiftCoastal, BerthStayHoursCoastal = @BerthStayHoursCoastal WHERE PDAEstimatorID = @PDAEstimatorID";
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
