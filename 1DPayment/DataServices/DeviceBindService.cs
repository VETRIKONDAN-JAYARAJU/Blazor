using DataObjects;
using Dapper;

namespace DataServices
{
    public class DeviceBindService : IGenericRepository<DeviceBind, int>
    {
        private readonly DBConnection connection;

        public DeviceBindService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<DeviceBind>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<DeviceBind>("SELECT devicebindid, device.devicename, merchant.merchantname, devicebind.bankid, bank.bankname, bankaccount.accountno, devicebind.botstatus, recordstatus.status FROM devicebind, device, merchant, bank, bankaccount, recordstatus WHERE devicebind.deviceid = device.deviceid AND devicebind.merchantid = merchant.merchantid AND devicebind.bankid = bank.bankid AND devicebind.bankaccountid = bankaccount.bankaccountid AND devicebind.statusid = recordstatus.statusid ORDER BY devicebindid");
            return result.ToList();
        }

        public async Task<List<DeviceBind>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<DeviceBind>("SELECT * FROM devicebind WHERE statusid = 1 ORDER BY devicebindid");
            return result.ToList();
        }

        public async Task<DeviceBind> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<DeviceBind>("SELECT * FROM devicebind WHERE devicebindid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(DeviceBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM devicebind WHERE deviceid = @DeviceId AND merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId", new { record.DeviceId, record.MerchantId, record.BankId, record.BankAccountId }).FirstOrDefault();

            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO devicebind (deviceid, merchantid, bankid, bankaccountid, createdby) VALUES (@DeviceId, @MerchantId, @BankId, @BankAccountId, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> UpdateBotStatus(DeviceBind record)
        {
            var db = connection.Create();
            await db.ExecuteAsync("UPDATE devicebind SET botstatus = @BotStatus WHERE devicebindid = @DeviceBindId", record);
            return true;
        }

        public async Task<bool> Update(DeviceBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM devicebind WHERE deviceid = @DeviceId AND merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId", new { record.DeviceId, record.MerchantId, record.BankId, record.BankAccountId }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT COUNT(*) FROM devicebind WHERE deviceid = @DeviceId AND merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId AND statusid=@StatusId", new { record.DeviceId, record.MerchantId, record.BankId, record.BankAccountId, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE devicebind SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE devicebindid = @DeviceBindId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE devicebind SET deviceid = @DeviceId, merchantid=@MerchantId, bankid = @BankId, bankaccountid=@BankAccountId, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE devicebindid = @DeviceBindId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO devicebind_auditlog (devicebindid, deviceid, merchantid, bankid, bankaccountid, createdby) VALUES (@DeviceBindId, @DeviceId, @MerchantId, @BankId, @BankAccountId, @CreatedBy)", record);

                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE devicebind SET statusid = 3 WHERE devicebindid = @DeviceBindId", new { DeviceBindId = key });
            return (rowsAffected > 0);
        }


        // Select Device Bind Information using MerchantId to Display in Merchant Form View Details

        public async Task<List<DeviceBind>> SelectByMerchantId(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<DeviceBind>("SELECT db.merchantid, me.merchantname, de.devicename, ba.bankname, bac.accountno, db.botstatus FROM devicebind db JOIN merchant me ON db.merchantid = me.merchantid JOIN device de ON db.deviceid = de.deviceid JOIN bank ba ON db.bankid = ba.bankid JOIN bankaccount bac ON db.bankaccountid = bac.bankaccountid WHERE db.merchantid = @key", new { key });
            return result.ToList();
        }


    }
}
