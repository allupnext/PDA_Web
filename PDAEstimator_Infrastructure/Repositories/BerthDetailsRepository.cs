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
    public class BerthDetailsRepository : IBerthDetailsRepository
    {
        private readonly IConfiguration configuration;
        public BerthDetailsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(BerthDetails entity)
        {
            var sql = "Insert into BerthDetails (BerthCode, BerthName, BerthStatus, MaxLoa, MaxBeam, MaxArrivalDraft,MaxDisplacement, DWT ,TerminalID, IsDeleted) VALUES (@BerthCode, @BerthName, @BerthStatus, @MaxLoa, @MaxBeam, @MaxArrivalDraft,@MaxDisplacement, @DWT,@TerminalID,0)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return new string(entity.ToString());
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "Update BerthDetails set IsDeleted = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<BerthDetails>> GetAllAsync()
        {
            var sql = "SELECT BerthDetails.*, TerminalDetails.ID as TerminalDetailsID, TerminalCode, TerminalName FROM BerthDetails left join TerminalDetails on TerminalDetails.ID = BerthDetails.TerminalID where BerthDetails.IsDeleted != 1";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<BerthDetails>(sql);
                return new List<BerthDetails>(result.ToList());
            }
        }

        public async Task<BerthDetails> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM BerthDetails WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<BerthDetails>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<List<BerthDetails>> GetByPortIdAsync(long id)
        {
            var sql = "select b.*, TerminalName from BerthDetails B left join TerminalDetails T on  t.ID=b.TerminalID where t.PortID= @Id AND b.IsDeleted=0";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<BerthDetails>(sql, new { Id = id });
                return new List<BerthDetails>(result.ToList());
            }
        }

        public async Task<int> UpdateAsync(BerthDetails entity)
        {
            var sql = "UPDATE BerthDetails SET BerthCode=@BerthCode, BerthName=@BerthName, BerthStatus=@BerthStatus, MaxLoa=@MaxLoa, MaxBeam=@MaxBeam, MaxArrivalDraft=@MaxArrivalDraft,MaxDisplacement = @MaxDisplacement, TerminalID=@TerminalID,DWT=@DWT WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
