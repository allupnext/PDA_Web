namespace PDAEstimator_Domain.Entities
{
    public  class PDAEstimatorOutPutTariff
    {
        public long PDAOutPutTariffID { get; set; }
        public long PDAEstimatorOutPutID { get; set; }
        public int ExpenseCategoryID  { get; set; }
        
        public string ChargeCodeName { get; set; }  
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal UNITS { get; set; }
        public string Remark { get; set; }
        public decimal Taxrate { get; set; }
        public int? TaxID { get; set; }

        public int? CurrencyID { get; set; }
    }
}
