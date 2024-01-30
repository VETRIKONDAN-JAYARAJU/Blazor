using DataObjects;
using Dapper;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DataServices
{
    public class FeeBindService : IGenericRepository<FeeBind, int>
    {
        private readonly DBConnection connection;

        public FeeBindService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<FeeBind>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<FeeBind>("SELECT feebindid, merchant.merchantname, paymenttype.paymenttype, currencycode, feerate, feebindtype, minamount, maxamount, limitamount, sourcemerchant, recordstatus.status FROM feebind, merchant, paymenttype, recordstatus WHERE feebind.merchantid = merchant.merchantid AND feebind.paymenttypeid = paymenttype.paymenttypeid AND feebind.statusid = recordstatus.statusid ORDER BY feebindid");
            return result.ToList();
        }

        public async Task<List<FeeBind>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<FeeBind>("SELECT * FROM feebind WHERE statusid = 1 ORDER BY feebindid");
            return result.ToList();
        }

        public async Task<FeeBind> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<FeeBind>("SELECT * FROM feebind WHERE feebindid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(FeeBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM feebind WHERE merchantid = @MerchantId AND paymenttypeid = @PaymentTypeId", new { record.MerchantId, record.PaymentTypeId }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO feebind (merchantid, paymenttypeid, currencycode, feerate, feebindtype, minamount, maxamount, limitamount, sourcemerchant, createdby) VALUES (@MerchantId, @PaymentTypeId, @CurrencyCode, @FeeRate, @FeeBindType, @MinAmount, @MaxAmount, @LimitAmount, @SourceMerchant, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(FeeBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM feebind WHERE merchantid = @MerchantId AND paymenttypeid = @PaymentTypeId AND currencycode=@CurrencyCode AND feerate=@FeeRate AND feebindtype=@FeeBindType AND minamount=@MinAmount AND maxamount=@MaxAMount AND limitamount=@LimitAmount AND sourcemerchant=@SourceMerchant", new { record.MerchantId, record.PaymentTypeId, record.CurrencyCode, record.FeeRate, record.FeeBindType, record.MinAmount, record.MaxAmount, record.LimitAmount, record.SourceMerchant }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT COUNT(*) FROM feebind WHERE merchantid = @MerchantId AND paymenttypeid = @PaymentTypeId AND statusid=@StatusId", new { record.MerchantId, record.PaymentTypeId, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE feebind SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE feebindid = @FeeBindId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE feebind SET merchantid=@MerchantId, paymenttypeid=@PaymentTypeId, currencycode=@CurrencyCode, feerate=@FeeRate, feebindtype=@FeeBindType, minamount=@MinAmount, maxamount=@MaxAMount, limitamount=@LimitAmount, sourcemerchant=@SourceMerchant, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE feebindid = @FeeBindId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO feebind_auditlog (feebindid, merchantid, paymenttypeid, currencycode, feerate, feebindtype, createdby) VALUES (@FeeBindId, @MerchantId, @PaymentTypeId, @CurrencyCode, @FeeRate, @FeeBindType, @CreatedBy)", record);

                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE feebind SET statusid = 3 WHERE feebindid = @FeeBindId", new { FeeBindId = key });
            return (rowsAffected > 0);
        }


        // Select FeeRate using MerchantId for Commission Calaculation
      
        public List<FeeBind> GetFeeRate(int key, string ccode)
        {
            List<FeeBind> result = new List<FeeBind>();
            var db = connection.Create();
            result = db.Query<FeeBind>("SELECT feerate FROM feebind WHERE statusid = 1 AND merchantid = @key AND currencycode = @ccode", new { key, ccode }).ToList();
            return result;
        }

        // Select Fee Bind Information using MerchantId to Display in Merchant Form View Details

        public async Task<List<FeeBind>> SelectByMerchantId(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<FeeBind>("SELECT fb.merchantid, me.merchantname AS merchantname, pt.paymenttype, fb.currencycode, fb.feerate, fb.feebindtype, fb.minamount, fb.maxamount, fb.limitamount, me1.merchantname AS sourcemerchantname FROM feebind fb JOIN merchant me ON fb.merchantid = me.merchantid JOIN paymenttype pt ON fb.paymenttypeid = pt.paymenttypeid JOIN merchant me1 ON fb.sourcemerchant = me1.merchantid WHERE fb.merchantid = @key", new { key });
            return result.ToList();
        }




    }
}