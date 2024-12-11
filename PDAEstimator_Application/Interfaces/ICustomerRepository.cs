using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<CustomerUserMaster> Authenticate(string email, string password);
        Task<List<CustomerList>> GetAlllistAsync();
        Task<string> AddCustomer_Company_MappingAsync(Company_Customer_Mapping entity);

        Task<int> DeleteCustomer_Company_MappingAsync(long id);

        Task<List<CustomerList>> GetAlllistCustomerAsync(int customerId);
        Task<CustomerUserMaster> CheckEmailExist(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(string token, int id);
        Task<string> ChangePassword(string Password, long id, string macAddress, string loginMachineName);
        Task<int> AuthenticateById(int id, string Password);
        Task<int> UpdateMacAddress(string MacAddress, int ID);
        Task<string> ChangePasswordByCurrent(string Password, long id);


    }
}
