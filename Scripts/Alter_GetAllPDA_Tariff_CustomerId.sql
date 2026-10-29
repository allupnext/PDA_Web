USE [DB_161786_hexaonreview]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Returns CustomerId (NULL = tariff applies to all customers)
-- =============================================
ALTER PROCEDURE [dbo].[GetAllPDA_Tariff]
 @PortId as Int,
 @ETA as datetime
AS
BEGIN
 SET NOCOUNT ON;

   SELECT TariffRateID, PortID, TerminalID, BerthID, CargoID, CallTypeID, TariffRate.ExpenseCategoryID, ChargeCodeID,
 SlabID, SlabFrom, SlabTo, Rate, CurrencyID, Validity_From, Validity_To, TariffRate.status, FormulaID, Remark,
 TaxID, ExpenseName, sequnce, ChargeCodeName,SlabIncreemental, VesselBallast, ChargeCodeMaster.Sequence as ChargeCodeSequence,TariffSegment.TariffSegmentName as SlabName, Range_TariffID, Reduced_GRT, OperationTypeID, TariffRate.CustomerId
 FROM TariffRate
 Left Join ExpenseMaster on ExpenseMaster.ID = TariffRate.ExpenseCategoryID and ExpenseMaster.IsDeleted != 1 and ExpenseMaster.Status = 1
 Left Join ChargeCodeMaster On ChargeCodeMaster.ID = TariffRate.ChargeCodeID and ChargeCodeMaster.IsDeleted != 1 and ChargeCodeMaster.Status = 1
 left join TariffSegment on TariffSegment.TariffSegmentID = TariffRate.SlabID
 where TariffRate.IsDeleted != 1  and Validity_From  <= @ETA and Validity_To >= @ETA and TariffRate.status = 1
 and PortID = @PortId
END
GO
