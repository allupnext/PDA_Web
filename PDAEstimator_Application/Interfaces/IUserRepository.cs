using PDAEstimator_Domain.Entities;
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
        Task<List<UserRolePermissionMenu>> GetAllUserRolePermissionMenuAsync();
        Task<List<UserRolePermissions>> GetAllUserRolePermissionsAsync();
        Task<string> AddUserPemissionRole_Role_MappingAsync(UserPemissionRole_Role_Mapping entity);
        Task<int> DeleteUserPemissionRole_Role_MappingAsync(int RoleID, int UserRolePermissionId);
        Task<List<UserPemissionRole_Role_Mapping>> GetAllUserPemissionRole_Role_Mapping();
        Task<User> CheckEmailExist(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(string token, int id);
        Task<string> ChangePassword(string Password, long id,string macAddress);
        Task<int> AddMacAddress(string MacAddress, long id);
        Task<int> AuthenticateById(int id, string Password);
        Task<int> UpdateOTP(string OTP, DateTime OTPSent, long Id);

        Task<int> UpdateLoginDetails(string LoginMachineName, DateTime LoginDateTime, long Id);

        Task<int> UpdateMacAddress(string MacAddress, int ID);
    }
}
