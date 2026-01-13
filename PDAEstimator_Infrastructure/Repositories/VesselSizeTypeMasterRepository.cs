using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class VesselSizeTypeMasterRepository : IVesselSizeTypeMasterRepository
    {
        private readonly IConfiguration _configuration;

        public VesselSizeTypeMasterRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AddAsync(VesselSizeTypeMaster entity)
        {
            try
            {
                var sql = "Insert into VesselSizeTypeMaster (VesselSizeTypeName,Description,DWT,GRT,NRT,LOA,Beam) VALUES (@VesselSizeTypeName,@Description,@DWT,@GRT,@NRT,@LOA,@Beam)";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return new string(entity.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            try
            {

                var sql = "Update VesselSizeTypeMaster set IsDeleted = 1  WHERE VesselSizeTypeId= @VesselSizeTypeId";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, new { VesselSizeTypeId = id });
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VesselSizeTypeMaster>> GetAllAsync()
        {
            try
            {

                var sql = "select * From VesselSizeTypeMaster where IsDeleted=0 order by VesselSizeTypeId desc";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<VesselSizeTypeMaster>(sql);
                    return new List<VesselSizeTypeMaster>(result.ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VesselSizeTypeMaster> GetByIdAsync(long id)
        {
            try
            {

                var sql = "select * From VesselSizeTypeMaster WHERE VesselSizeTypeId= @VesselSizeTypeId";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<VesselSizeTypeMaster>(sql, new { VesselSizeTypeId = id });
                    return result ?? new VesselSizeTypeMaster ();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<int> UpdateAsync(VesselSizeTypeMaster entity)
        {
            try
            {
                var sql = "Update VesselSizeTypeMaster set VesselSizeTypeName = @VesselSizeTypeName,Description=@Description,DWT=@DWT,GRT=@GRT,NRT=@NRT,LOA=@LOA,Beam=@Beam WHERE VesselSizeTypeId = @VesselSizeTypeId AND IsDeleted = 0";
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(sql, entity);
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
