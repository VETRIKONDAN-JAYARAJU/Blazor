using DataObjects;
using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DataServices
{
    public class MerchantService
    {
        private readonly DBConnection connection;

        public MerchantService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Merchant>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Merchant>("SELECT me.merchantid, me.merchantname, me.apiurl, bk.bankname, dk.devicekey, me1.merchantname AS uppermerchantname, me.merchanttype, me.commission, me.operationmode, rs.status FROM merchant me JOIN bank bk ON me.bankid = bk.bankid JOIN device dk ON me.deviceid = dk.deviceid LEFT JOIN merchant me1 ON me.uppermerchant = me1.merchantid JOIN recordstatus rs ON me.statusid = rs.statusid ORDER BY me.merchantid");
            return result.ToList();
        }

        public async Task<List<Merchant>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Merchant>("SELECT merchantid, merchantname FROM merchant WHERE statusid = 1 ORDER BY merchantid");
            return result.ToList();
        }
        
        public async Task<List<Merchant>> SelectDepositMerchant()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Merchant>("SELECT merchantid, merchantname FROM merchant WHERE statusid = 1 AND merchantid > 1 ORDER BY merchantid");
            return result.ToList();
        }


        public async Task<List<Merchant>> SelectActiveExceptTop() 
        {
            int merchantCount = GetMerchantCount();

            if (merchantCount == 1)
            {
                var db = connection.Create();
                var result = await db.QueryAsync<Merchant>("SELECT merchantid, merchantname FROM merchant WHERE statusid = 1 AND merchantid = 1 ORDER BY merchantid");
                return result.ToList();
            }
            else
            {
                var db = connection.Create();
                var result = await db.QueryAsync<Merchant>("SELECT merchantid, merchantname FROM merchant WHERE statusid = 1 AND merchantid >= 1 ORDER BY merchantid");
                return result.ToList();
            }
        }

        public int GetMerchantCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM merchant WHERE statusid = 1");
            return result;
        }

        public int GetUpperMerchant(int key)
        {
            var db = connection.Create();
            var result = db.QueryFirst("SELECT uppermerchant FROM merchant WHERE merchantid = @key", new { key });
            return result.uppermerchant;
        }

        public decimal GetMerchantCommission(int key)
        {
            var db = connection.Create();
            var result = db.QueryFirst("SELECT commission FROM merchant WHERE merchantid = @key", new { key });
            return result.commission;
        }

        public decimal GetUpperMerchantCommission(int key)
        {
            var db = connection.Create();
            var result = db.QueryFirst("SELECT commission FROM merchant WHERE merchantid = @key", new { key });
            return result.commission;
        }

        public decimal GetCurrentMerchantBalance(int key)
        {
            var db = connection.Create();
            var result = db.QueryFirst("SELECT BalanceAmount FROM merchantbalance WHERE merchantid = @key", new { key });
            return result.balanceamount;
        }


        public void UpdateUpperMerchantCommission()
        {
           // decimal currentMerchantBalance = GetCurrentMerchantBalance();


        }



        public async Task<List<MerchantBalances>> SelectMerchantBalance(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBalances>("SELECT * FROM MerchantBalance WHERE merchantid = @key", new { key });
            return result.ToList();
        }


        public async Task<Merchant> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Merchant>("SELECT * FROM merchant WHERE merchantid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Merchant record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM merchant WHERE merchantname = Trim(@MerchantName)", new { record.MerchantName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                record.UpperMerchant = (record.UpperMerchant == 0) ? 0 : record.UpperMerchant;

                await db.ExecuteAsync(@"INSERT INTO merchant (merchantname, apiurl, bankid, deviceid, uppermerchant, merchanttype, commission, operationmode, createdby) VALUES (@MerchantName, @ApiUrl, @BankId, @DeviceId, @UpperMerchant, @MerchantType, @Commission, @OperationMode, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Merchant record)
        {
            var db = connection.Create();
            // var isExist = db.Query<bool>("SELECT count(*) FROM merchant WHERE merchantname = Trim(@MerchantName)", new { record.MerchantName }).FirstOrDefault();

            var isExist = db.Query<bool>("SELECT count(*) FROM merchant WHERE merchantname = Trim(@MerchantName) AND apiurl=@ApiUrl AND bankid=@BankId AND deviceid=@DeviceId AND uppermerchant=@UpperMerchant AND merchanttype=@MerchantType AND commission=@Commission AND operationmode=@OperationMode", new { record.MerchantName, record.ApiUrl, record.BankId, record.DeviceId, record.UpperMerchant, record.MerchantType, record.Commission, record.OperationMode }).FirstOrDefault();


            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM merchant WHERE merchantname = Trim(@MerchantName) AND statusid=@StatusId", new { record.MerchantName, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE merchant SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE merchantid = @MerchantId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE merchant SET merchantname=@MerchantName, apiurl=@ApiUrl, bankid=@BankId, deviceid=@DeviceId, uppermerchant=@UpperMerchant, merchanttype=@MerchantType, commission=@Commission, operationmode=@OperationMode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE merchantid = @MerchantId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO merchant_auditlog (merchantid, merchantname, apiurl, bankid, deviceid, uppermerchant, merchanttype, commission, createdby) VALUES (@MerchantId, @MerchantName, @ApiUrl, @BankId, @DeviceId, @UpperMerchant, @MerchantType, @Commission, @CreatedBy)", record);

                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE merchant SET statusid = 3 WHERE merchantid = @MerchantId", new { MerchantId = key });
            return (rowsAffected > 0);
        }


        // Select Amount using MerchantId for Commission Calaculation

        public decimal GetBalanceAmount(int key)
        {
            var db = connection.Create();
            var result = db.QueryFirst("SELECT balance FROM merchant WHERE statusid = 1 AND merchantid = @key", new { key });
            return result.balance;
        }

        public bool UpdateMerchantNewBalance(int MerchantId, decimal MerchantBalance, decimal EarnCommission)
        {
            var db = connection.Create();
            db.ExecuteAsync("UPDATE merchant SET balance=@MerchantBalance, commission=@EarnCommission WHERE merchantid = @MerchantId", new { MerchantId, MerchantBalance, EarnCommission });
            return true;
        }


    }
}