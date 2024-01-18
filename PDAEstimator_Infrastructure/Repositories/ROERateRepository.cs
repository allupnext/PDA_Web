using Dapper;
using Microsoft.Extensions.Configuration;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure.Repositories
{
    public class ROERateRepository : IROERateRepository
    {
        private readonly IConfiguration configuration;
        public ROERateRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> AddAsync(ROERates entity)
        {

            var listSelectedROE = entity.ROEinsertedvalue.ToList();
            var listSelectedROEName = entity.ROENameinsertedvalue.ToList();
            var listSelectedCurrency = entity.Currencyinsertedvalue.ToList();
            var sql = "DELETE FROM ROEMaster WHERE CreationDate = CONVERT(date, GETDATE())";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
            }

           
            int num = 0;
            int count = 0;
            int currencycnt = listSelectedCurrency.Count / listSelectedROEName.Count;
            for (int j = 0; j < listSelectedROEName.Count; j++)
            {
                for (int i = num; i < listSelectedROE.Count; i++)
                {
                    if (count < currencycnt)
                    {
                        var sql1 = "Insert into ROEMaster (CurrencyId,ROERate,UserId,CreationDate,Status,ROEDate,RoenameID ) VALUES (" + listSelectedCurrency[i].ToString() + "," + listSelectedROE[i].ToString() + ",1,CONVERT(date, GETDATE()),1,CONVERT(date, GETDATE())," + listSelectedROEName[j].ToString() + ")";
                        using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                        {
                            connection.Open();
                            var result = await connection.ExecuteAsync(sql1, entity);
                            //return new string(entity.ToString());

                        }
                        num = num + 1;
                        count= count + 1;
                    }
                }
                count = 0;
            }



            return new string(entity.ToString());


        }


        public async Task<List<ROERates>> GetAllAsync()
        {
            //var sql = "select C.id,CurrencyName,isnull(ROERate,0) as ROERate from Currency C left join ROEMaster R on C.Id=R.currencyid and CreationDate=(select max(CreationDate) from ROEMaster) order by CurrencyName";
            var sql = "select C.id,CurrencyName,0 as ROERate from Currency C left join ROEMaster R on C.Id=R.currencyid and CreationDate=(select max(CreationDate) from ROEMaster) where c.IsDeleted != 1 and  C.BaseCurrency != 1 Group by C.id,CurrencyName order by CurrencyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ROERates>(sql);
                return new List<ROERates>(result.ToList());
            }
        }
        public async Task<List<ROERates>> GetAlllistbyDate(DateTime searchDate)
        {
            if (searchDate == null)
            {
                var sql = "select C.id,CurrencyName,isnull(ROERate,0) as ROERate ,ro.ROEName from Currency C left join ROEMaster R on C.Id=R.currencyid left join ROENameMaster ro on r.RoenameID=ro.ID where CreationDate=(select max(CreationDate) from ROEMaster) order by ro.ROEName,CurrencyName";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<ROERates>(sql);
                    return new List<ROERates>(result.ToList());
                }
            }
            else
            {
                var sql = "select C.id,CurrencyName,isnull(ROERate,0) as ROERate ,ro.ROEName from Currency C left join ROEMaster R on C.Id=R.currencyid and CONVERT(date, CreationDate)=CONVERT(date,'" + searchDate + "' ) left join ROENameMaster ro on r.RoenameID=ro.ID  order by ro.ROEName,CurrencyName";
                using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<ROERates>(sql);
                    return new List<ROERates>(result.ToList());
                }
            }
            return null;

        }

        public async Task<List<ROERates>> GetAlllistbyLoadAll()
        {
            var sql = "select C.id,CurrencyName,isnull(ROERate,0) as ROERate ,ro.ROEName from Currency C left join ROEMaster R on C.Id=R.currencyid left join ROENameMaster ro on r.RoenameID=ro.ID where CreationDate=(select max(CreationDate) from ROEMaster) order by ro.ROEName,CurrencyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ROERates>(sql);
                return new List<ROERates>(result.ToList());
            }

        }

        public async Task<List<ROERates>> GetAlllistbyLoadAllforDefaultRoEName()
        {
            var sql = "select C.id,CurrencyName,isnull(ROERate,0) as ROERate ,ro.ROEName from Currency C left join ROEMaster R on C.Id=R.currencyid left join ROENameMaster ro on r.RoenameID=ro.ID where CreationDate=(select max(CreationDate) from ROEMaster) and ro.ID = (SELECT ID from ROENameMaster where IsDefault = 1)  order by ro.ROEName,CurrencyName";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ROERates>(sql);
                return new List<ROERates>(result.ToList());
            }

        }


        public async Task<ROERates> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM ROEMaster WHERE ID = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<ROERates>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> DeleteAsync(long id)
        {
            var sql = "DELETE FROM ROEMaster WHERE Id = @Id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }
        public async Task<int> UpdateAsync(ROERates entity)
        {
            var sql = "UPDATE ROEMaster SET CurrencyId = @CurrencyId, ROERate = @ROERate ,UserId=@UserId,CreationDate=@CreationDate,Status=@Status  WHERE ID = @Id";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
