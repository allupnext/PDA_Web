using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PDAEstimatorOutPutNote
    {
        public long PDAEstimatorOutPutNoteID { get; set; }
        public long PDAEstimatorOutPutID { get; set; }  
        public string Note { get; set;   }
        public int sequnce { get; set; }
    }
}
