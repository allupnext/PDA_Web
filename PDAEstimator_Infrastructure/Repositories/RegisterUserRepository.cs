using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        public Task<string> AddAsync(Customer entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
