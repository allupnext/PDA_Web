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
    public class TariffMasterRepository : ITariffMasterRepository
    {

        private readonly IConfiguration configuration;
        public TariffMasterRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(TariffMaster entity)
        {
            try
            {
                var sql = "Insert into TariffMaster (PortID , IsDeleted) VALUES (@PortID, 0)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result =  connection.QuerySingle<int>(sql, entity);
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
            var sql = "Update TariffMaster set IsDeleted = 1 WHERE TariffID = @TariffID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { TariffID = id });
                return result;
            }
        }

        public async Task<List<TariffMaster>> GetAllAsync()
        {
            var sql = "SELECT * FROM TariffMaster where  IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TariffMaster>(sql);
                return new List<TariffMaster>(result.ToList());
            }
        }

        public async Task<List<TariffMasterList>> GetAllTariffMasterAsync()
        {
            var sql = "SELECT TariffMaster.TariffID as TariffID, TariffMaster.PortID as PortID, PortName, (select count(*) from TariffRate where TariffMaster.TariffID = TariffRate.TariffID) TariffCount FROM TariffMaster left join PortDetails on TariffMaster.PortID = PortDetails.ID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TariffMasterList>(sql);
                return new List<TariffMasterList>(result.ToList());
            }
        }

        public async Task<List<TariffRateList>> GetAllTariffRateAsync(int PortId)
        {
            try
            {
                //var sql = "SELECT tariffrate.ModifyUserID as ModifyUserID,ModifyUser.EmployCode as ModifyUser, tariffrate.CreatedBy as UserId,CreatedByUser.EmployCode as CreatedBy ,TariffRate.PortID as PortID, PortName, Validity_From, Validity_To, tariffRate.TerminalID as TerminalID,TariffRate.status as status, TerminalName, BerthID, BerthName, CargoID, CargoName, CallTypeID, CallTypeName, tariffRate.ExpenseCategoryID as ExpenseCategoryID, ExpenseName, ChargeCodeID, ChargeCodeName, Rate, CurrencyID, CurrencyCode,tariffRate.TariffRateID, SlabID, TariffSegment.TariffSegmentName as SlabName, SlabFrom, SlabTo, FormulaID, TaxID,TaxName, Remark , SlabIncreemental , VesselBallast ,TariffRate.CreationDate,TariffRate.ModifyDate FROM TariffRate left join UserMaster as ModifyUser on tariffrate.ModifyUserID = ModifyUser.ID left join UserMaster as CreatedByUser on tariffrate.CreatedBy = CreatedByUser.ID  left join PortDetails on tariffRate.PortID = PortDetails.ID left join TerminalDetails on tariffRate.TerminalID = TerminalDetails.ID left join BerthDetails on tariffRate.BerthID = BerthDetails.ID left join CargoDetails on tariffRate.CargoID = CargoDetails.ID left join CallType on tariffRate.CallTypeID = CallType.ID left join ExpenseMaster on tariffRate.ExpenseCategoryID = ExpenseMaster.ID left join ChargeCodeMaster on tariffRate.ChargeCodeID = ChargeCodeMaster.ID left join Currency on tariffRate.CurrencyID = Currency.ID left join TariffSegment on TariffSegment.TariffSegmentID = tariffRate.SlabID left join TaxMaster on TaxMaster.ID = tariffRate.TaxID where TariffRate.PortId = @PortId and TariffRate.IsDeleted != 1 ORDER BY TariffRateID desc";
                //var sql = "SELECT tariffrate.ModifyUserID as ModifyUserID,ModifyUser.EmployCode as ModifyUser, tariffrate.CreatedBy as UserId,CreatedByUser.EmployCode as CreatedBy ,TariffRate.PortID as PortID, PortName, Validity_From, Validity_To, tariffRate.TerminalID as TerminalID,TariffRate.status as status, TerminalName, BerthID, BerthName, CargoID, CargoName, CallTypeID, CallTypeName, tariffRate.ExpenseCategoryID as ExpenseCategoryID, ExpenseName, ChargeCodeID, ChargeCodeName, Rate, CurrencyID, CurrencyCode,tariffRate.TariffRateID, SlabID, TariffSegment.TariffSegmentName as SlabName, SlabFrom, SlabTo, FormulaID, TaxID,TaxName, Remark , SlabIncreemental , VesselBallast,Reduced_GRT ,TariffRate.CreationDate,TariffRate.ModifyDate FROM TariffRate left join UserMaster as ModifyUser on tariffrate.ModifyUserID = ModifyUser.ID left join UserMaster as CreatedByUser on tariffrate.CreatedBy = CreatedByUser.ID left join PortDetails on tariffRate.PortID = PortDetails.ID left join TerminalDetails on tariffRate.TerminalID = TerminalDetails.ID left join BerthDetails on tariffRate.BerthID = BerthDetails.ID left join CargoDetails on tariffRate.CargoID = CargoDetails.ID left join CallType on tariffRate.CallTypeID = CallType.ID left join ExpenseMaster on tariffRate.ExpenseCategoryID = ExpenseMaster.ID left join ChargeCodeMaster on tariffRate.ChargeCodeID = ChargeCodeMaster.ID left join Currency on tariffRate.CurrencyID = Currency.ID left join TariffSegment on TariffSegment.TariffSegmentID = tariffRate.SlabID left join TaxMaster on TaxMaster.ID = tariffRate.TaxID where TariffRate.PortId = @PortId and TariffRate.IsDeleted != 1 ORDER BY TariffRateID desc";
                //var sql = "SELECT tariffrate.ModifyUserID as ModifyUserID,ModifyUser.EmployCode as ModifyUser, tariffrate.CreatedBy as UserId,CreatedByUser.EmployCode as CreatedBy ,TariffRate.PortID as PortID, PortName, Validity_From, Validity_To, tariffRate.TerminalID as TerminalID,TariffRate.status as status,TariffRate.OperationTypeID as OperationTypeID, TerminalName, BerthID, BerthName, CargoID, CargoName, CallTypeID, CallTypeName, tariffRate.ExpenseCategoryID as ExpenseCategoryID, ExpenseName, ChargeCodeID, ChargeCodeName, Rate, CurrencyID, CurrencyCode,tariffRate.TariffRateID, SlabID, TariffSegment.TariffSegmentName as SlabName, SlabFrom, SlabTo,Range_TariffID, FormulaID, TaxID,TaxName, Remark , SlabIncreemental , VesselBallast,Reduced_GRT ,TariffRate.CreationDate,TariffRate.ModifyDate FROM TariffRate left join UserMaster as ModifyUser on tariffrate.ModifyUserID = ModifyUser.ID left join UserMaster as CreatedByUser on tariffrate.CreatedBy = CreatedByUser.ID left join PortDetails on tariffRate.PortID = PortDetails.ID left join TerminalDetails on tariffRate.TerminalID = TerminalDetails.ID left join BerthDetails on tariffRate.BerthID = BerthDetails.ID left join CargoDetails on tariffRate.CargoID = CargoDetails.ID left join CallType on tariffRate.CallTypeID = CallType.ID left join ExpenseMaster on tariffRate.ExpenseCategoryID = ExpenseMaster.ID left join ChargeCodeMaster on tariffRate.ChargeCodeID = ChargeCodeMaster.ID left join Currency on tariffRate.CurrencyID = Currency.ID left join TariffSegment on TariffSegment.TariffSegmentID = tariffRate.SlabID left join TaxMaster on TaxMaster.ID = tariffRate.TaxID where TariffRate.PortId = @PortId and TariffRate.IsDeleted != 1 ORDER BY TariffRateID desc";
                var sql = "GetAllTariffByPortId";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<TariffRateList>(sql, new { PortId = PortId });
                    return new List<TariffRateList>(result.ToList());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        public async Task<TariffMaster> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM TariffMaster WHERE TariffID = @TariffID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TariffMaster>(sql, new { TariffID = id });
                return result;
            }
        }

        public async Task<TariffMaster> GetByPortIdAsync(int PortId)
        {
            var sql = "SELECT * FROM TariffMaster WHERE PortId = @PortId";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TariffMaster>(sql, new { PortId = PortId });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TariffMaster entity)
        {
            var sql = "UPDATE TariffMaster SET PortID=@PortID, TerminalID=@TerminalID,BerthID=@BerthID,CargoID=@CargoID,CallTypeID=@CallTypeID WHERE TariffID = @TariffID";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
