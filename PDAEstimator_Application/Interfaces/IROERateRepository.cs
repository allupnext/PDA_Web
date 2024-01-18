using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IROERateRepository : IGenericRepository<ROERates>
    {
        Task<List<ROERates>> GetAlllistbyDate(DateTime ssearchDate);
        Task<List<ROERates>> GetAlllistbyLoadAll();

        Task<List<ROERates>> GetAlllistbyLoadAllforDefaultRoEName();

    }
}
