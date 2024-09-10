using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IFormulaTransactionRepository : IGenericRepository<FormulaTransaction>
    {
        Task<List<FormularTransList>> GetAllTransAsync(int formulaID);
        Task<List<FormularTransList>> GetAllTransAsync();
        Task<int> DeleteByFormulaIdAsync(int formulaID);
        Task<int> DeleteFormulaIdAsync(int formulaID);

    }
}
