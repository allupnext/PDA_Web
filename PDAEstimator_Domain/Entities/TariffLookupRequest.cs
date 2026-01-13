using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TariffLookupRequest
    {
        public string? Port { get; set; }
        public string? Terminal { get; set; }
        public string? Berth { get; set; }
        public string? Cargo { get; set; }
        public string? OperationType { get; set; }
        public string? CallType { get; set; }
        public string? Expense { get; set; }
        public string? ChargeCode { get; set; }
        public string? Tax { get; set; }
        public string? Slab { get; set; }
        public string? Currency { get; set; }
        public string? Formula { get; set; }
    }

}
