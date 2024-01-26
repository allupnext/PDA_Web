﻿using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<UserList>> GetAlllistAsync();

        Task<User> Authenticate(string user , string password);

        Task<string> AddCustomer_User_MappingAsync(Company_User_Mapping entity);
        Task<int> DeleteCustomer_User_MappingAsync(long id);

        Task<int> DeletePort_User_MappingAsync(long id);
        Task<User> GetFullUserByIdAsync(long id);
        Task<User> GetAllUsersById(long id);
        Task<string> AddPort_User_MappingAsync(User_Port_Mapping entity);
    }
}
