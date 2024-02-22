using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IBankMasterRepository : IGenericRepository<BankMaster>
    {
        Task<List<BankMaster>> GetAllBankDetailsAsync();
        Task<BankMaster> GetByCompanyIdAsync(int id);
    }
}
