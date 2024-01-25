using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface ITariffRateRepository : IGenericRepository<TariffRate>
    {
        Task<int> DeleteByTariffIDAsync(int tariffID);
        Task<string> InsertTarrifFromSelectedPorts(CopyTarrifModelInput Ids);
        Task<string> InsertTarrifFromSamePorts(CopyTarrifModelInput Ids);
    }
}
