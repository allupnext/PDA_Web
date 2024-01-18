using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface ITariffMasterRepository : IGenericRepository<TariffMaster>
    {
        Task<List<TariffMasterList>> GetAllTariffMasterAsync();
        Task<TariffMaster> GetByPortIdAsync(int PortId);
        Task<List<TariffRateList>> GetAllTariffRateAsync(int PortId);

    }   
}
