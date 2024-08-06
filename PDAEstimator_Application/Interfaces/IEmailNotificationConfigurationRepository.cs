using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IEmailNotificationConfigurationRepository : IGenericRepository<EmailNotificationConfiguration>
    {
        Task<EmailNotificationConfiguration> GetByProcessNameAsync(string ProcessName);
        Task<EmailNotificationConfiguration> GetByCompanyandProcessNameAsync(int CompanyId, string ProcessName);
    }
}
