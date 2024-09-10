using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface ICustomerUserMaster : IGenericRepository<CustomerUserMaster>
    {

        Task<List<CustomerUserMaster>> GetByCustomerIdAsync(long id);

        Task<List<CustomerUserMaster>> GetCustomerUserByEmailAsync(string email);
        Task<int> AddMacAddress(string MacAddress, long id);
        Task<int> UpdateOTP(string OTP, DateTime OTPSent, long Id);

        Task<int> UpdateLoginDetails(string LoginMachineName, DateTime LoginDateTime, long Id);
    }
}
