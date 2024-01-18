using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class CargoHandledRepsitory : ICargoHandledRepsitory
    {
        private readonly IConfiguration configuration;
        public CargoHandledRepsitory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(CargoHandleds entity)
        {
            try
            {
                    var sql = "Insert into CargoHandled (PortID,TerminalID,BerthID,CargoID,CargoStatus, IsDeleted) VALUES (@PortID,@TerminalID, @BerthID,@CargoID,@CargoStatus, 0)";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return new string(entity.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update CargoHandled set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<CargoHandleds>> GetAllAsync()
        {
            var sql = "SELECT * FROM CargoHandled where IsDeleted! = 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<CargoHandleds>(sql);
                return new List<CargoHandleds>(result.ToList());
            }
        }


        public async Task<List<CargoHandledList>> GetAlllistAsync()
        {
            try
            {
                var sql = "SELECT CargoHandled.ID as ID,CargoHandled.TerminalID as TerminalID,CargoHandled.BerthID as BerthID,CargoHandled.PortID as PortID,CargoHandled.CargoID as CargoID,CargoHandled.CargoStatus as CargoStatus,TerminalDetails.TerminalName,BerthDetails.BerthName,PortDetails.PortName,CargoDetails.CargoName FROM CargoHandled left join TerminalDetails on TerminalDetails.ID =  CargoHandled.TerminalID left join BerthDetails on BerthDetails.ID =  CargoHandled.BerthID left join PortDetails on PortDetails.ID =  CargoHandled.PortID left join CargoDetails on CargoDetails.ID =  CargoHandled.CargoID WHERE CargoHandled.IsDeleted != 1;";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<CargoHandledList>(sql);
                    return new List<CargoHandledList>(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<CargoHandleds> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM CargoHandled WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CargoHandleds>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(CargoHandleds entity)
        {
            var sql = "UPDATE CargoHandled SET PortID=@PortID,TerminalID = @TerminalID,BerthID=@BerthID, CargoID=@CargoID,CargoStatus=@CargoStatus  WHERE ID = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
