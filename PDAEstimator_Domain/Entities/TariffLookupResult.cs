using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities;

public class TariffLookupResult
{
    public class TariffLookupAllResult
    {
        public int PortID { get; set; }
        public int? TerminalID { get; set; }
        public int BerthID { get; set; }
        public int? CargoID { get; set; }
        public int? OperationTypeID { get; set; }
        public int? CallTypeID { get; set; }
        public int ExpenseCategoryID { get; set; }
        public int ChargeCodeID { get; set; }
        public int? TaxID { get; set; }
        public int? SlabID { get; set; }
        public int? CurrencyID { get; set; }
        public int? FormulaID { get; set; }
    }

}
