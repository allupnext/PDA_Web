using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IPDAEstimitorRepository :IGenericRepository<PDAEstimator>
    {
        Task<List<CargoDetails>> GetCargoByTerminalAndPortAsync(int terminalId, int portId);
        Task<List<TerminalDetails>> GetTerminalByCargoIdAndPortAsync(int CargoID, int PortID);
        Task<List<PDAEstimatorList>> GetAlllistAsync();
        Task<List<PDATariffRateList>> GetAllPDA_Tariff(int portId, DateTime ETA);
        Task<List<Notes>> GetNotes();
        Task<Company_Customer_Mapping> GetbyCustIdasync(int id);
        Task<List<PDAEstimatorList>> GetPDAEstiomatorListOfLast30Days();
        Task<List<PDAEstimatorList>> GetAlllistByCustIdAsync(int CustomerID);

    }
}
