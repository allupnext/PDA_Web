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
    public class TariffRateRepository : ITariffRateRepository
    {

        private readonly IConfiguration configuration;
        public TariffRateRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(TariffRate entity)
        {
            try
            {

                var sql = "Insert into TariffRate (PortID, TerminalID, CurrencyID, CreationDate, CreatedBy, BerthID, CallTypeID, CargoID,ExpenseCategoryID, SlabID, SlabFrom, SlabTo, Rate,status, ChargeCodeID, Validity_From,Validity_To, FormulaID, IsDeleted, Remark, TaxID, SlabIncreemental, VesselBallast, Reduced_GRT, Range_TariffID,OperationTypeID) VALUES (@PortID, @TerminalID, @CurrencyID, GetDate(), @CreatedBy, @BerthID, @CallTypeID, @CargoID,@ExpenseCategoryID,@SlabID,@SlabFrom, @SlabTo, @Rate,@status, @ChargeCodeID, @Validity_From, @Validity_To, @FormulaID, 0, @Remark, @TaxID, @SlabIncreemental, @VesselBallast, @Reduced_GRT, @Range_TariffID,@OperationTypeID) SELECT CAST(SCOPE_IDENTITY() as int)";
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
        public async Task<string> InsertTarrifFromSelectedPorts(CopyTarrifModelInput Ids)
        {
            try
            {
                //var sql = "Insert into TariffRate (PortID, TerminalID, CurrencyID, BerthID, CallTypeID, CargoID,ExpenseCategoryID, SlabID, SlabFrom, SlabTo, Rate,status, ChargeCodeID, Validity_From,Validity_To, FormulaID, IsDeleted, Remark, TaxID, SlabIncreemental, VesselBallast) VALUES (@PortID, @TerminalID, @CurrencyID, @BerthID, @CallTypeID, @CargoID,@ExpenseCategoryID,@SlabID,@SlabFrom, @SlabTo, @Rate,@status, @ChargeCodeID, @Validity_From, @Validity_To, @FormulaID, 0, @Remark, @TaxID, @SlabIncreemental, @VesselBallast) SELECT CAST(SCOPE_IDENTITY() as int)";
                var sql = "insert into [dbo].[TariffRate](PortID, TerminalID, CurrencyID, CreationDate, BerthID, CallTypeID, CargoID,ExpenseCategoryID, SlabID, SlabFrom, SlabTo, Rate,status, ChargeCodeID, Validity_From,Validity_To, FormulaID, IsDeleted, Remark, TaxID, SlabIncreemental, VesselBallast, Reduced_GRT, Range_TariffID)  select " + Ids.CopyFromportid + ",TerminalID, CurrencyID,GETDATE(), BerthID, CallTypeID, CargoID,ExpenseCategoryID, SlabID, SlabFrom, SlabTo, Rate,status, ChargeCodeID, Validity_From,Validity_To, FormulaID, IsDeleted, Remark, TaxID, SlabIncreemental, VesselBallast, Reduced_GRT, Range_TariffID  from [dbo].[TariffRate]  where PortId = " + Ids.CopyToportid + " and (CallTypeID = " + Ids.CopyCallTypetid + " Or  "+ Ids.CopyCallTypetid +" = 0) and IsDeleted=0";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, Ids);
                    return new string(Ids.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> InsertTarrifFromSamePorts(CopyTarrifModelInput Ids)
        {
            try
            {
                //var sql = "Insert into TariffRate (PortID, TerminalID, CurrencyID, BerthID, CallTypeID, CargoID,ExpenseCategoryID, SlabID, SlabFrom, SlabTo, Rate,status, ChargeCodeID, Validity_From,Validity_To, FormulaID, IsDeleted, Remark, TaxID, SlabIncreemental, VesselBallast) VALUES (@PortID, @TerminalID, @CurrencyID, @BerthID, @CallTypeID, @CargoID,@ExpenseCategoryID,@SlabID,@SlabFrom, @SlabTo, @Rate,@status, @ChargeCodeID, @Validity_From, @Validity_To, @FormulaID, 0, @Remark, @TaxID, @SlabIncreemental, @VesselBallast) SELECT CAST(SCOPE_IDENTITY() as int)";
                var sql = "Insert Into TariffRate( PortID,TerminalID,BerthID,CargoID,CallTypeID,ExpenseCategoryID,ChargeCodeID,SlabID,SlabFrom,SlabTo,Rate,CurrencyID,CreatedBy,CreationDate,IPAddress,ModifyUserID,ModifyDate,Validity_From,Validity_To,status,FormulaID,IsDeleted,Remark,TaxID,SlabIncreemental,VesselBallast,Reduced_GRT, Range_TariffID ) select " + Ids.CopyFromportid + " ,TerminalID,BerthID,CargoID, " + Ids.ToCallTypeId + ",ExpenseCategoryID,ChargeCodeID,SlabID,SlabFrom,SlabTo,Rate,CurrencyID,CreatedBy,CreationDate,IPAddress,ModifyUserID,ModifyDate,Validity_From,Validity_To,status,FormulaID,IsDeleted,Remark,TaxID,SlabIncreemental,VesselBallast,Reduced_GRT, Range_TariffID from TariffRate where CallTypeId = " + Ids.FromCallTypeId +" and  PortID = " +Ids.CopyFromportid;
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, Ids);
                    return new string(Ids.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update TariffRate set IsDeleted = 1 WHERE TariffRateID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> DeleteByTariffIDAsync(int tariffID)
        {
            try
            {
                var sql = "DELETE FROM TariffRate WHERE TariffRateID = @Id";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { Id = tariffID });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<TariffRate>> GetAllAsync()
        {
            var sql = "SELECT * FROM TariffRate where  IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TariffRate>(sql);
                return new List<TariffRate>(result.ToList());
            }
        }

        public async Task<TariffRate> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM TariffRate WHERE TariffRateID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TariffRate>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TariffRate entity)
        {
            try
            {
                var sql = "UPDATE TariffRate SET PortID = @PortID, TerminalID=@TerminalID,CurrencyID=@CurrencyID,ModifyDate= GetDate(), ModifyUserID = @ModifyUserID,BerthID=@BerthID,CallTypeID=@CallTypeID,CargoID=@CargoID,ExpenseCategoryID=@ExpenseCategoryID, SlabID=@SlabID, SlabFrom=@SlabFrom, SlabTo = @SlabTo,Rate=@Rate,ChargeCodeID=@ChargeCodeID,Validity_From=@Validity_From, Validity_To=@Validity_To,status=@status, FormulaID = @FormulaID, Remark = @Remark, TaxID = @TaxID, SlabIncreemental = @SlabIncreemental, VesselBallast = @VesselBallast, Reduced_GRT = @Reduced_GRT, Range_TariffID = @Range_TariffID, OperationTypeID = @OperationTypeID WHERE TariffRateID = @TariffRateID";

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
