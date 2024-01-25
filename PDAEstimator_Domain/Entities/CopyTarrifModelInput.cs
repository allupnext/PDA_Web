using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CopyTarrifModelInput
    {
        public int CopyFromportid { get; set;}
        public int CopyToportid { get; set;}
        public int CopyCallTypetid { get; set;}
        public int FromCallTypeId { get; set;}
        public int ToCallTypeId { get; set;}

    }
}
